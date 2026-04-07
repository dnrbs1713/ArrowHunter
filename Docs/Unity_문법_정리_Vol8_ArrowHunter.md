# Unity C# 문법 정리 Vol.8
ArrowHunter 프로젝트 구조와 전투 리팩터링에서 배운 문법

이 문서는 현재 `ArrowHunter` 프로젝트에 들어 있는 스크립트 구조와, 최근에 전투 구조를 정리하면서 같이 배운 Unity / C# 문법을 한 번에 보기 쉽게 정리한 문서다.

이번 문서의 목표는 아래 3가지다.

1. 지금 프로젝트가 어떤 구조로 돌아가는지 빠르게 이해하기
2. `BattleData`, `BattleDataManager`, `TurnManager`처럼 헷갈리기 쉬운 책임을 정확히 구분하기
3. Unity 문법을 단순 이론이 아니라 현재 프로젝트 코드 흐름 기준으로 이해하기

## 1. ScriptableObject와 런타임 인스턴스

ArrowHunter에서 가장 중요한 데이터 분리는 `원본 데이터`와 `실행 중 데이터`를 나누는 것이다.

- `BaseStatSO`는 원본 데이터다.
- `PlayerInstance`는 실행 중 변하는 데이터다.

예를 들어 플레이어의 기본 체력, 공격력, 방어력, 프리팹, 스킬 목록은 `BaseStatSO`에 들어간다.
반면 현재 체력, 버프 보너스, 아이템 보너스처럼 게임 도중 바뀌는 값은 `PlayerInstance`에 들어간다.

```csharp
public class BaseStatSO : ScriptableObject
{
    public string jobName;
    public GameObject prefab;
    public int maxHp;
    public int attackDamage;
    public float defensePower;
    public int maxCost;
    public int startCost;
    public int baseCostRecovery;
    public List<SkillSO> skillList;
}

public class PlayerInstance
{
    public int currentHp;
    public int bonusAttack;
    public int bonusMaxHp;
    public float bonusDefense;
}
```

왜 이렇게 나누는가?

- `BaseStatSO`는 에셋이라 여러 씬에서 공통으로 재사용하기 쉽다.
- 런타임 중 값을 바꾸면 원본 에셋이 오염될 수 있으므로 실행 중 데이터는 따로 빼는 편이 안전하다.
- 플레이어는 전투가 끝나도 현재 체력을 유지해야 하므로 `PlayerInstance` 같은 런타임 객체가 필요하다.

정리:

- `BaseStatSO` = 설계도
- `PlayerInstance` = 실제 플레이 도중 바뀌는 상태

## 2. static 싱글톤 + DontDestroyOnLoad

현재 프로젝트에는 씬이 바뀌어도 계속 살아 있어야 하는 매니저가 있다.

대표적으로:

- `GameSceneManager`
- `BattleDataManager`
- `RoomManager`

이런 매니저는 보통 아래 패턴으로 작성한다.

```csharp
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

여기서 중요한 문법 포인트는 2개다.

### `static`

`static` 필드는 클래스 전체에서 하나만 존재한다.
그래서 `GameSceneManager.instance`, `TurnManager.instance`처럼 어디서든 접근할 수 있다.

### `DontDestroyOnLoad`

씬 전환 후에도 오브젝트를 파괴하지 않고 유지한다.

왜 필요한가?

- `BattleDataManager`는 맵 씬에서 받은 적 데이터를 배틀 씬에서도 가지고 있어야 한다.
- `RoomManager`는 방문한 방 정보를 맵 씬을 다시 열어도 유지해야 한다.
- `GameSceneManager`는 전투 승패 이벤트를 듣고 씬을 넘겨야 한다.

주의:

- `DontDestroyOnLoad`만 쓰고 중복 제거를 안 하면 씬 전환 때마다 매니저가 하나씩 더 생긴다.
- 그래서 `if (instance != null) Destroy(gameObject);` 패턴이 거의 항상 같이 들어간다.

## 3. event + Action으로 느슨하게 연결하기

현재 프로젝트에서는 이벤트를 꽤 잘 쓰고 있다.

대표 예시는 2가지다.

1. `MonsterEncounter.OnEncountMonster`
2. `TurnManager.OnVictory`, `TurnManager.OnDefeat`

예:

```csharp
public static event Action<BaseStatSO, bool> OnEncountMonster;
public static event Action<int> OnVictory;
public static event Action OnDefeat;
```

이벤트의 장점은 발행자와 구독자를 느슨하게 연결할 수 있다는 점이다.

예를 들어 `MonsterEncounter`는 그냥

```csharp
OnEncountMonster?.Invoke(monsterStat, isBoss);
```

만 호출하면 된다.

그러면 누가 듣고 있든 상관없이:

- `BattleDataManager`는 적 데이터를 저장하고
- 나중에 다른 시스템이 필요하면 그 시스템도 같은 이벤트를 구독하면 된다

즉 발행자는 “누가 받을지”를 몰라도 된다.

### `?.Invoke()`

```csharp
OnDefeat?.Invoke();
```

여기서 `?.`는 null 조건부 연산자다.

- 구독자가 하나도 없으면 이벤트는 null일 수 있다.
- 그냥 `OnDefeat.Invoke()`를 하면 null 예외가 난다.
- `?.Invoke()`를 쓰면 null일 때 안전하게 무시된다.

### 구독과 해제

이벤트는 보통 `OnEnable`, `OnDisable`에서 관리한다.

```csharp
private void OnEnable()
{
    TurnManager.OnVictory += ShowVictory;
    TurnManager.OnDefeat += ShowDefeat;
}

private void OnDisable()
{
    TurnManager.OnVictory -= ShowVictory;
    TurnManager.OnDefeat -= ShowDefeat;
}
```

이 패턴을 지키는 이유:

- 씬 전환 시 자동 정리된다
- 비활성화되면 중복 구독을 막을 수 있다
- 메모리 누수와 Missing Reference 문제를 줄일 수 있다

## 4. BattleData와 BattleDataManager의 책임 분리

이 부분은 ArrowHunter에서 가장 헷갈리기 쉬운 지점이다.

원칙은 아주 단순하게 잡는 게 좋다.

### `BattleData`

맵 복귀용 데이터만 저장한다.

현재 책임:

- `isVictory`
- `turnCount`
- `playerMapPosition`
- `enterDirection`
- `currentRoomWorldPos`

즉 “전투 끝난 뒤 맵으로 돌아갈 때 필요한 값”만 넣는다.

### `BattleDataManager`

전투에 들어갈 때 필요한 런타임 데이터를 저장한다.

현재 책임:

- `playerStatSO`
- `PlayerInstance`
- `CurrentEnemyStat`
- `IsBossBattle`

즉 “이번 전투를 시작하기 위해 필요한 값”은 여기로 몰아주는 게 맞다.

### 왜 분리해야 하나?

예전 구조처럼 `BattleData`에 전투용 적 정보까지 같이 넣으면:

- 맵 복귀 데이터와 전투 진입 데이터가 섞인다
- 어떤 씬이 어떤 값을 읽어야 하는지 헷갈린다
- 책임이 모호해져서 버그가 생기기 쉽다

정리:

- `BattleData` = 맵 복귀 데이터
- `BattleDataManager` = 전투 시작 데이터

## 5. 상태 패턴: BattleState / PlayerAttackState / EnemyTurnState

전투 흐름을 `if-else` 덩어리 하나로 관리하면 금방 복잡해진다.
그래서 현재 프로젝트는 상태 패턴을 쓰고 있다.

```csharp
public abstract class BattleState
{
    protected TurnManager manager;

    public BattleState(TurnManager manager) => this.manager = manager;

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
```

이 구조의 핵심은 “현재 턴 상태가 무엇인지”를 클래스 단위로 분리하는 것이다.

### `PlayerAttackState`

- 플레이어 턴 시작
- 방향키 입력 감지
- 스턴 등 행동불가 상태 체크

### `EnemyTurnState`

- 적 공격 방향 결정
- 플레이어 방어 입력 대기

### 왜 좋은가?

- 플레이어 턴과 적 턴 로직을 따로 읽을 수 있다
- 나중에 `VictoryState`, `DefeatState`, `SkillInputState` 같은 상태를 추가하기 쉽다
- 턴 흐름을 메서드 하나에 몰아 넣지 않아도 된다

하지만 현재 구조에서는 상태 클래스가 입력만 담당하는 것이 아니라, 실제 계산은 여전히 `TurnManager`가 많이 가져가고 있다.
그래서 상태 패턴은 도입됐지만, 책임 분리는 아직 덜 끝난 상태라고 볼 수 있다.

## 6. 상속보다 조합: BattleEntity + Controller

처음 보면 “플레이어와 적이면 `BattleEntity`를 상속받는 클래스가 있어야 하는 것 아닌가?”라고 생각하기 쉽다.
하지만 현재 구조는 상속형이 아니라 조합형이다.

즉 이런 구조다.

- `BattleEntity` = 체력, 스탯, 상태이상, 피격 처리 담당
- `PlayerCombatController` = 플레이어 입력 담당
- `EnemyController` = 적 방향 선택 담당

같은 오브젝트에 같이 붙여서 사용한다.

```csharp
public class PlayerCombatController : MonoBehaviour
{
    public BattleEntity playerEntity;
}

public class EnemyController : MonoBehaviour
{
    public BattleEntity enemyEntity;
}
```

왜 이런 구조가 좋은가?

- 본체 데이터와 조작 로직이 분리된다
- 컨트롤러만 바꾸는 식으로 재사용하기 쉽다
- AI 적과 플레이어가 같은 `BattleEntity`를 공유할 수 있다

즉:

- 상속 = “너는 BattleEntity의 한 종류다”
- 조합 = “너는 BattleEntity를 가지고 행동한다”

현재 ArrowHunter에는 조합 구조가 더 잘 맞는다.

## 7. Instantiate + GetComponent + 프리팹 구조

배틀 씬에서 가장 중요한 런타임 흐름은 `BattleSpawner`다.

```csharp
GameObject obj = Instantiate(enemySO.prefab, transform.position, transform.rotation);
BattleEntity entity = obj.GetComponent<BattleEntity>();
EnemyController controller = obj.GetComponent<EnemyController>();
```

이 코드는 문법상 단순하지만, 실제로 자주 문제가 생긴다.

### 왜 자주 터지나?

`Instantiate()`는 프리팹을 복제할 뿐이다.
프리팹 안에 필요한 컴포넌트가 정확히 붙어 있지 않으면 이후 로직이 전부 실패한다.

예를 들어 몬스터 프리팹에:

- `BattleEntity`가 없거나
- `EnemyController`가 없거나
- 같은 컴포넌트가 중복되면

`TurnManager.RegisterEnemy()`가 호출되지 못하고, 결과적으로:

- 코스트 UI가 안 뜨고
- 입력도 안 먹고
- `Battle initialized` 로그도 안 뜨게 된다

### `GetComponent()`와 `GetComponentInChildren()`

#### `GetComponent<T>()`

현재 오브젝트에서만 찾는다.

```csharp
var entity = obj.GetComponent<BattleEntity>();
```

#### `GetComponentInChildren<T>()`

자식 오브젝트까지 포함해서 찾는다.

```csharp
var entity = obj.GetComponentInChildren<BattleEntity>();
```

프리팹 구조가 단순하면 `GetComponent()`가 더 명확하다.
프리팹 루트가 복잡하거나 자식에 본체가 들어 있으면 `GetComponentInChildren()`이 더 안전할 수 있다.

## 8. NullReferenceException 디버깅 패턴

Unity에서 가장 많이 보는 오류 중 하나가 `NullReferenceException`이다.
ArrowHunter에서도 배틀 씬 UI와 전투 초기화 과정에서 자주 등장했다.

가장 중요한 원칙은 “무조건 null 체크를 늘리는 것”보다 “정확히 무엇이 null인지 로그로 좁히는 것”이다.

예:

```csharp
Debug.Log($"player = {player}");
Debug.Log($"enemy = {enemy}");
Debug.Log($"playerController = {playerController}");
Debug.Log($"enemyController = {enemyController}");
Debug.Log($"player stat = {(player != null ? player.statData : null)}");
Debug.Log($"enemy stat = {(enemy != null ? enemy.statData : null)}");
```

이렇게 찍어보면 아래 같은 식으로 바로 원인을 좁힐 수 있다.

- `player`만 있고 `enemy`가 없다
- `enemy`는 있는데 `enemyController`만 없다
- 둘 다 있는데 `enemy.statData`가 null이다

### UI에서의 null 체크

`BattleUIManager` 같은 경우는 초기화 타이밍이 엇갈릴 수 있으므로 아래처럼 방어 코드를 넣는 편이 안전하다.

```csharp
if (TurnManager.instance == null) return;
if (TurnManager.instance.player == null || TurnManager.instance.enemy == null) return;
if (costText == null) return;
```

주의:

- UI가 안 뜬다고 무조건 UI 문제는 아니다
- 실제로는 `TurnManager` 초기화가 안 끝나서 코스트 값이 준비되지 않은 경우도 많다

## 9. Coroutine과 턴 흐름

ArrowHunter 전투에서는 공격 후 잠깐 대기하는 흐름이 필요하다.
그래서 코루틴을 사용한다.

```csharp
StartCoroutine(PlayerAttackSequence(playerDir));

private IEnumerator PlayerAttackSequence(Direction playerDir)
{
    isProcessing = true;

    // 공격 판정

    yield return new WaitForSeconds(0.5f);

    isProcessing = false;
}
```

### `IEnumerator`

코루틴 메서드는 `IEnumerator`를 반환해야 한다.

### `yield return`

Unity에게 “여기서 잠깐 멈췄다가 다음 프레임 또는 일정 시간 뒤에 다시 이어서 실행해라”라고 말하는 문법이다.

### 왜 코루틴이 필요한가?

- 공격 로그를 보여준 뒤 바로 다음 턴으로 넘어가면 너무 딱딱하다
- 0.5초 정도 기다리면 전투 흐름이 훨씬 자연스럽다
- 상태 전환 타이밍을 시각적으로 맞추기 쉽다

### `WaitForSeconds`와 `WaitForSecondsRealtime`

둘의 차이도 중요하다.

```csharp
yield return new WaitForSeconds(0.5f);
yield return new WaitForSecondsRealtime(0.5f);
```

- `WaitForSeconds`는 `Time.timeScale` 영향을 받는다
- `WaitForSecondsRealtime`는 실제 시간 기준이다

그래서 승패 UI에서 `Time.timeScale = 0f`를 썼다면,
씬 전환 대기에는 `WaitForSecondsRealtime`를 써야 한다.

## 10. TurnManager가 무거워지는 이유

현재 `TurnManager`는 이름과 달리 너무 많은 일을 하고 있다.

지금 맡고 있는 일을 나눠보면:

- 엔티티 등록
- 전투 시작 준비 확인
- 상태 전환
- 공격 실행
- 방어 실행
- 콤보 처리
- 스킬 사용
- 코스트 사용과 회복
- 승패 이벤트 발행

이 정도면 “턴 관리자”라기보다 전투 전체 실행기다.

그래서 코드가 커질수록 아래 문제가 생긴다.

- 입력 문제 하나를 고치려 해도 전투 전체를 같이 봐야 한다
- 상태 로직과 계산 로직이 섞인다
- UI도 `TurnManager` 내부 상태에 강하게 의존하게 된다

## 11. 추천 리팩터링: BattleRuntime + BattleActionService

현재 구조에서 가장 자연스러운 분리 방향은 이렇다.

### `TurnManager`

남길 책임:

- 상태 전환
- 턴 시작/종료
- 승패 이벤트
- 전투 시작 타이밍 관리

### `BattleRuntime`

역할:

- `Player`
- `Enemy`
- `PlayerController`
- `EnemyController`

를 모아 관리하고, 전투 준비가 끝났는지 `IsReady`로 판단한다.

예:

```csharp
public bool IsReady =>
    Player != null &&
    Enemy != null &&
    PlayerController != null &&
    EnemyController != null &&
    Player.statData != null &&
    Enemy.statData != null;
```

### `BattleActionService`

역할:

- 플레이어 공격 처리
- 적 공격 처리
- 콤보 처리
- 스킬 사용
- 코스트 사용 및 회복

즉 “무슨 계산이 일어나는가”는 이 서비스가 담당하고,
“지금 누구 턴인가”는 `TurnManager`가 담당하는 구조다.

### 왜 이 분리가 좋은가?

- `TurnManager`가 훨씬 짧고 읽기 쉬워진다
- 상태 클래스는 입력만 읽고 실행은 서비스에 넘길 수 있다
- 코스트와 콤보를 한 곳에서 관리하므로 버그 추적이 쉬워진다

## 12. 프로퍼티와 표현식 멤버

ArrowHunter 코드에는 프로퍼티 문법이 많이 나온다.

예:

```csharp
public int currentCost => _costHandler.currentCost;
public int maxCost => _costHandler.maxCost;
```

이건 아래와 같은 의미다.

```csharp
public int currentCost
{
    get { return _costHandler.currentCost; }
}
```

즉 `=>`를 이용한 읽기 전용 프로퍼티 축약형이다.

장점:

- 짧고 읽기 쉽다
- 계산식이 간단할 때 특히 유용하다

## 13. `?`, `?.`, `??` 문법

ArrowHunter에서 자주 헷갈리는 null 관련 문법도 같이 정리해두면 좋다.

### null 조건부 연산자 `?.`

```csharp
OnVictory?.Invoke(turnCount);
```

의미:

- `OnVictory`가 null이 아니면 `Invoke()`
- null이면 아무 것도 하지 않음

### null 병합 연산자 `??`

```csharp
var skills = runtimeSkills ?? new List<SkillSO>();
```

의미:

- `runtimeSkills`가 null이 아니면 그것을 사용
- null이면 오른쪽 값을 사용

### null 체크와 조합

```csharp
if (enemySO?.prefab == null) return;
```

이 문장은 꽤 자주 쓰이는 패턴이다.

의미:

- `enemySO`가 null이면 true
- `enemySO.prefab`이 null이어도 true
- 둘 중 하나라도 null이면 return

즉 두 단계 null 체크를 아주 짧게 쓸 수 있다.

## 14. 프리팹과 씬에서 꼭 기억할 체크리스트

배틀 씬이 꼬일 때는 아래를 순서대로 확인하면 된다.

### 플레이어 프리팹

- `BattleEntity`
- `PlayerCombatController`

### 몬스터 프리팹

- `BattleEntity`
- `EnemyController`

### 배틀 씬

- `TurnManager`
- `BattleUIManager`
- 플레이어용 `BattleSpawner`
- 적용 `BattleSpawner`

### UI

- `playerHpBar`
- `enemyHpBar`
- `costText`

이 참조가 모두 연결되어 있는지 확인

### 디버그 기준 로그

- `RegisterPlayer`
- `RegisterEnemy`
- `Battle initialized`

이 3개가 차례대로 찍히면 전투 시작 흐름은 거의 정상이라고 볼 수 있다.

## 15. 마지막 정리

현재 ArrowHunter 구조를 가장 짧게 요약하면 이렇다.

- 맵 씬은 방과 이동을 관리한다
- `MonsterEncounter`가 전투 진입을 시작한다
- `BattleDataManager`가 전투 시작 데이터를 들고 간다
- `BattleSpawner`가 실제 전투 참가자를 만든다
- `TurnManager`가 턴 상태를 관리한다
- `BattleUIManager`가 UI를 갱신한다

그리고 앞으로 구조를 더 좋게 만들려면 아래 원칙을 계속 확인하면 된다.

1. 이 데이터는 원본인가, 런타임 값인가?
2. 이 값은 맵 복귀용인가, 전투 시작용인가?
3. 이 로직은 턴 흐름인가, 액션 계산인가?
4. 이 클래스 이름과 실제 책임이 맞는가?

이 4가지만 계속 체크해도 구조가 훨씬 안정적으로 잡힌다.

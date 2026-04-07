# ArrowHunter 게임 구조 정리

이 문서는 현재 `ArrowHunter` 프로젝트의 스크립트 구조와, 최근에 정리한 전투 리팩터링 방향을 한 번에 보기 쉽게 정리한 문서다.

목표는 아래 3가지다.

- 지금 프로젝트에서 각 스크립트가 무슨 역할을 하는지 빠르게 파악하기
- `BattleData`, `BattleDataManager`, `TurnManager`처럼 헷갈리기 쉬운 책임을 구분하기
- Unity에서 자주 쓰는 문법과 흐름을 이 프로젝트 기준으로 이해하기

## 1. 프로젝트 큰 흐름

현재 게임 흐름은 크게 아래 순서로 진행된다.

1. 맵 씬에서 방을 생성하고 플레이어를 배치한다.
2. `MonsterEncounter`가 플레이어 충돌을 감지한다.
3. `BattleDataManager`가 이번 전투에 필요한 적 데이터를 저장한다.
4. `GameSceneManager`가 배틀 씬으로 전환한다.
5. 배틀 씬에서 `BattleSpawner`가 플레이어/적 프리팹을 생성한다.
6. `TurnManager`가 전투 준비 완료를 확인하고 턴 상태를 시작한다.
7. 전투 결과에 따라 맵 씬 또는 결과 씬으로 이동한다.

한 줄로 줄이면:

`Map -> Encounter -> BattleDataManager 세팅 -> BattleScene -> Spawn -> Turn Start -> Result/Map`

## 2. 주요 폴더 구조

### `Assets/02.Scripts/Common`

- 공용 enum, 계산기, 비용 계산기, 공통 SO가 모여 있다.
- 예:
  - `BaseStatSO`
  - `CostHandler`
  - `CombatProcessor`

### `Assets/02.Scripts/Data`

- 씬을 넘나드는 런타임 데이터 저장용 스크립트가 있다.
- 예:
  - `BattleDataManager`
  - `PlayerInstance`

### `Assets/02.Scripts/MapSceneScripts`

- 맵 씬에서 방 생성, 문 이동, 몬스터 배치, 조우 처리 등을 담당한다.
- 예:
  - `MapSceneInit`
  - `RoomManager`
  - `MonsterSpawner`
  - `MonsterEncounter`

### `Assets/02.Scripts/BattleSceneScripts/02.Scripts/Battle`

- 배틀 상태, 턴 흐름, 전투 판정, 스폰 관련 스크립트가 있다.
- 예:
  - `TurnManager`
  - `BattleSpawner`
  - `BattleResolver`
  - `PlayerAttackState`
  - `EnemyTurnState`

### `Assets/02.Scripts/BattleSceneScripts/02.Scripts/Entity`

- 전투 오브젝트 본체와 조작용 컨트롤러가 있다.
- 예:
  - `BattleEntity`
  - `PlayerCombatController`
  - `EnemyController`

### `Assets/02.Scripts/BattleSceneScripts/02.Scripts/UI`

- HP 바, 코스트, 승패 UI를 관리한다.
- 예:
  - `BattleUIManager`
  - `ResultSceneUI`

## 3. 데이터 구조 정리

### `BaseStatSO`

역할:

- 캐릭터/몬스터의 원본 데이터 보관
- 프리팹, HP, 공격력, 방어력, 코스트, 스킬 목록 저장

중요 포인트:

- `ScriptableObject`라서 에디터에서 재사용하기 좋다.
- 플레이어와 적 모두 같은 타입으로 다룰 수 있다.

예시 필드:

- `jobName`
- `prefab`
- `maxHp`
- `attackDamage`
- `defensePower`
- `skillList`

### `PlayerInstance`

역할:

- 플레이어의 런타임 상태를 보관
- 버프, 아이템, 현재 체력처럼 전투 중 바뀌는 값을 저장

중요 포인트:

- `BaseStatSO`는 원본 데이터
- `PlayerInstance`는 실행 중 변하는 실제 데이터

### `BattleData`

역할:

- 맵 복귀용 임시 데이터 저장

현재 책임:

- `isVictory`
- `turnCount`
- `playerMapPosition`
- `enterDirection`
- `currentRoomWorldPos`

중요 포인트:

- 전투용 적 정보는 더 이상 여기에 넣지 않는 쪽이 맞다.
- `BattleData`는 맵 복귀와 결과 전달용으로 최소화하는 것이 좋다.

### `BattleDataManager`

역할:

- 이번 전투에 필요한 런타임 전투 데이터를 보관

현재 책임:

- `playerStatSO`
- `PlayerInstance`
- `CurrentEnemyStat`
- `IsBossBattle`

중요 포인트:

- `MonsterEncounter` 이벤트를 받아 적 데이터를 세팅한다.
- `DontDestroyOnLoad`로 씬을 넘어도 유지된다.
- 전투 준비용 데이터 저장소 역할을 한다.

정리:

- `BattleData` = 맵 복귀 데이터
- `BattleDataManager` = 전투 진입 데이터

## 4. 맵 씬 구조

### `RoomManager`

역할:

- 현재 방 상태 관리
- 방문한 방 기록
- 방 이동
- 문 잠금/해제 처리

핵심 책임:

- 현재 방 좌표와 월드 좌표 관리
- `OnRoomChanged` 이벤트 발행
- 방 클리어 처리

### `MapSceneInit`

역할:

- 맵 씬이 시작될 때 현재 방을 실제 오브젝트로 생성/복원
- 플레이어 위치 재설정
- 몬스터 스포너 호출

흐름:

1. 현재 방 오브젝트 생성 또는 재활성화
2. `RoomManager`에 등록
3. 전투 승리 후라면 방 클리어 반영
4. 플레이어 위치 복원
5. 몬스터 스폰

### `MonsterEncounter`

역할:

- 플레이어가 몬스터와 충돌했을 때 전투 진입 시작

핵심 흐름:

1. 플레이어 위치를 `BattleData.playerMapPosition`에 저장
2. `OnEncountMonster(stat, isBoss)` 이벤트 발생
3. `BattleDataManager`가 적 데이터를 저장
4. `GameSceneManager.LoadBattle()` 호출

## 5. 배틀 씬 구조

### `BattleSpawner`

역할:

- 배틀 씬 입장 시 플레이어/적 프리팹을 생성

중요 포인트:

- 플레이어는 `BattleDataManager.PlayerInstance`를 사용해 초기화
- 적은 `BattleDataManager.CurrentEnemyStat`를 사용해 초기화
- 생성 후 `TurnManager`에 등록해야 전투가 시작된다

주의:

- 프리팹 루트 또는 탐색 가능한 위치에 `BattleEntity`, `PlayerCombatController`, `EnemyController`가 있어야 한다.
- 이 연결이 실패하면 `Battle initialized`가 되지 않고 입력/코스트/UI가 함께 꼬일 수 있다.

### `BattleEntity`

역할:

- 전투 참가자 공통 본체

책임:

- `statData`
- `currentHp`
- `statusHandler`
- `Initialize()`
- `TakeDamage()`
- `OnDie()`

중요 포인트:

- 플레이어/적 모두 이 컴포넌트를 중심으로 다뤄진다.
- `PlayerCombatController`나 `EnemyController`는 상속 구조가 아니라 조합 구조다.

정리:

- `BattleEntity` = 본체
- `PlayerCombatController` / `EnemyController` = 조작 로직

### `TurnManager`

현재 역할:

- 엔티티 등록
- 전투 시작 가능 여부 확인
- 상태 전환
- 플레이어 공격/적 공격 처리
- 콤보 처리
- 스킬 사용 처리
- 승패 이벤트 발생

문제점:

- 이름은 `TurnManager`인데 실제로는 전투 계산과 실행까지 많이 떠안고 있다.
- 턴 흐름만 관리해야 하는데 액션 계산, 코스트, 콤보, 스킬까지 섞여 있다.

### `BattleState`, `PlayerAttackState`, `EnemyTurnState`

역할:

- 상태 패턴으로 턴 흐름을 분리

현재 구조:

- `PlayerAttackState`는 플레이어 방향키 입력을 읽는다.
- `EnemyTurnState`는 적 공격을 보여주고 플레이어 방어 입력을 읽는다.
- 실제 공격 계산은 아직 `TurnManager`가 많이 들고 있다.

### `BattleUIManager`

역할:

- HP 바, 코스트, 승패 패널 갱신

주의:

- `TurnManager.instance`
- `player`, `enemy`
- `costText`
- `playerHpBar`, `enemyHpBar`

이런 참조가 아직 준비되지 않았을 수 있으므로 null 체크가 중요하다.

## 6. 현재 구조에서 자주 발생하는 문제

### 1. 배틀 씬 진입 후 코스트가 안 뜨고 입력이 안 먹는 경우

가능성 높은 원인:

- `TurnManager.TryInitializeBattle()`가 끝까지 못 감
- 플레이어나 적 등록 중 하나 실패
- `playerController` 또는 `enemyController`가 null
- 적 프리팹에 `BattleEntity` 또는 `EnemyController`가 없음

확인 순서:

1. `Battle initialized` 로그가 뜨는지 확인
2. `RegisterPlayer`, `RegisterEnemy` 로그 확인
3. 프리팹 루트 컴포넌트 확인

### 2. UI NullReferenceException

가능성 높은 원인:

- `BattleUIManager` 참조 누락
- `costText` 또는 HP 바 미연결
- `TurnManager.player/enemy`가 아직 등록되지 않음

해결 원칙:

- UI 갱신 메서드에서 null 체크
- 씬의 Missing Script 제거

### 3. The referenced script (Unknown) on this Behaviour is missing!

의미:

- 씬이나 프리팹에 끊어진 MonoBehaviour 참조가 남아 있음

대응:

1. 해당 오브젝트 찾기
2. 정말 필요한 스크립트인지 확인
3. 현재 구조에 필요 없으면 제거

예:

- `PlayerHP`, `EnemyHP`, `PlayerCost` 오브젝트에 예전 스크립트 참조가 남아 있을 수 있음
- 현재 구조에서는 `BattleUIManager`가 직접 갱신하므로 `Image`와 `TextMeshProUGUI`만 있어도 충분한 경우가 많다

## 7. 추천 리팩터링 구조

현재 가장 큰 문제는 `TurnManager`가 너무 많은 책임을 갖고 있다는 점이다.

추천 구조는 아래와 같다.

### `TurnManager`

남길 책임:

- 상태 전환
- 턴 시작/종료
- 승패 이벤트
- 전투 시작 타이밍 관리

빼야 할 책임:

- 공격 계산
- 적/플레이어 액션 실행
- 콤보 판정
- 스킬 실행
- 코스트 사용/회복 세부 로직

### `BattleRuntime`

역할:

- 플레이어, 적, 컨트롤러 등록
- 전투 준비 완료 여부 판단

예상 책임:

- `Player`
- `Enemy`
- `PlayerController`
- `EnemyController`
- `IsReady`

### `BattleActionService`

역할:

- 플레이어 공격 처리
- 적 공격 처리
- 스킬 사용
- 콤보 상태
- 코스트 처리

예상 책임:

- `TryPlayerAttack`
- `TryEnemyAttack`
- `TryUseSkill`
- `EndPlayerTurn`

### 리팩터링 후 흐름

1. `BattleSpawner`가 엔티티 생성
2. `BattleRuntime`에 플레이어/적 등록
3. `TurnManager`가 `runtime.IsReady`를 확인
4. `BattleActionService.Initialize()` 호출
5. `TurnManager`는 상태 전환만 담당
6. 상태 클래스는 입력만 읽고, 실제 실행은 `BattleActionService`에 위임

## 8. Unity 문법 정리

이 프로젝트에서 자주 나오는 Unity 문법만 골라서 정리하면 아래와 같다.

### `MonoBehaviour`

Unity 컴포넌트 기본 클래스다.

예:

```csharp
public class TurnManager : MonoBehaviour
{
}
```

의미:

- GameObject에 붙일 수 있다.
- `Awake`, `Start`, `Update` 같은 Unity 이벤트 함수를 사용할 수 있다.

### `Awake`, `Start`, `Update`

#### `Awake()`

- 오브젝트가 생성될 때 가장 먼저 호출
- 싱글톤 세팅, 참조 캐싱에 자주 사용

예:

```csharp
private void Awake()
{
    instance = this;
}
```

#### `Start()`

- `Awake` 이후 한 번 호출
- 실제 게임 시작 로직이나 초기화 시작에 사용

예:

```csharp
private void Start()
{
    SpawnUnit();
}
```

#### `Update()`

- 매 프레임 호출
- 입력 처리나 프레임 단위 갱신에 사용

예:

```csharp
private void Update()
{
    RefreshHpBars();
}
```

### `OnEnable`, `OnDisable`, `OnDestroy`

이벤트 구독/해제에 자주 쓴다.

예:

```csharp
private void OnEnable()
{
    TurnManager.OnVictory += ShowVictory;
}

private void OnDisable()
{
    TurnManager.OnVictory -= ShowVictory;
}
```

원칙:

- 구독한 곳에서 반드시 해제도 해주는 습관이 중요하다.

### `SerializeField`

private 필드를 인스펙터에 노출하고 싶을 때 사용한다.

예:

```csharp
[SerializeField] private Image playerHpBar;
```

장점:

- 외부 직접 접근은 막고
- 인스펙터 연결은 가능하다

### `GetComponent` / `GetComponentInChildren`

같은 오브젝트 또는 자식 오브젝트의 컴포넌트를 찾을 때 사용한다.

예:

```csharp
var entity = GetComponent<BattleEntity>();
var controller = GetComponentInChildren<EnemyController>();
```

구분:

- `GetComponent<T>()` = 현재 오브젝트에서 찾기
- `GetComponentInChildren<T>()` = 자식까지 포함해서 찾기

### `Instantiate`

프리팹을 런타임에 생성할 때 사용한다.

예:

```csharp
GameObject obj = Instantiate(prefab, transform.position, transform.rotation);
```

### `ScriptableObject`

데이터 자산을 코드로 표현하는 방식이다.

이 프로젝트에서는 `BaseStatSO`, `SkillSO` 등이 여기에 해당한다.

장점:

- 프리팹과 분리된 데이터 관리
- 재사용성 높음
- 밸런스 조정이 쉬움

### `Action` 이벤트

여러 스크립트 사이를 느슨하게 연결할 때 쓴다.

예:

```csharp
public static event Action<int> OnVictory;
public static event Action<BaseStatSO, bool> OnEncountMonster;
```

장점:

- 직접 참조 없이도 알림 전달 가능

주의:

- 구독 해제를 안 하면 메모리 누수나 중복 호출이 생길 수 있다.

### `Coroutine`

시간 지연이 필요한 흐름에 사용한다.

예:

```csharp
StartCoroutine(PlayerAttackSequence(playerDir));
```

언제 쓰는가:

- 공격 연출 후 0.5초 대기
- 씬 전환 전 잠깐 기다리기

### `DontDestroyOnLoad`

씬이 바뀌어도 오브젝트를 유지하고 싶을 때 쓴다.

이 프로젝트 예:

- `GameSceneManager`
- `BattleDataManager`
- `RoomManager`

주의:

- 싱글톤 중복 생성 방지 로직과 같이 써야 한다.

## 9. 이 프로젝트에서 기억하면 좋은 설계 원칙

### 1. 데이터와 로직을 분리하기

- `BaseStatSO`는 원본 데이터
- `PlayerInstance`는 런타임 데이터
- `BattleEntity`는 전투 본체

### 2. 맵 복귀 데이터와 전투 진입 데이터를 분리하기

- `BattleData`는 맵 복귀용
- `BattleDataManager`는 전투 진입용

### 3. 상태 클래스는 입력과 흐름만 다루기

- `PlayerAttackState`
- `EnemyTurnState`

이 둘은 입력을 읽고 다음 호출만 넘기는 쪽이 깔끔하다.

### 4. 매니저 이름과 실제 책임을 맞추기

`TurnManager`가 전투 전체 계산을 다 들고 있으면 이름과 책임이 어긋난다.

원칙:

- `TurnManager` = 턴 흐름
- `BattleActionService` = 액션 실행
- `BattleRuntime` = 전투 참가자 데이터

### 5. 프리팹 구조를 일정하게 유지하기

플레이어/적 프리팹에는 필요한 핵심 컴포넌트가 정확히 있어야 한다.

예:

- 플레이어 프리팹
  - `BattleEntity`
  - `PlayerCombatController`

- 몬스터 프리팹
  - `BattleEntity`
  - `EnemyController`

중복되거나 빠져 있으면 디버깅이 매우 어려워진다.

## 10. 추천 다음 작업

우선순위 기준으로 정리하면 아래 순서가 좋다.

1. `TurnManager`에서 등록/실행 책임 분리
2. `BattleRuntime` 추가
3. `BattleActionService` 추가
4. `PlayerAttackState`, `EnemyTurnState`를 입력 중심으로 단순화
5. UI, 프리팹, 이벤트 흐름 안정화

추가로 나중에 하면 좋은 것:

- `RoomManager` 인코딩/주석 정리
- 디버그 로그 정리
- 배틀 초기화 실패 시 더 친절한 에러 로그 추가
- 프리팹 검증 체크리스트 문서화

---

이 문서는 현재 코드 기준의 구조 설명 + 최근에 정리한 리팩터링 방향을 함께 담고 있다.
실제 구현 중에는 이 문서를 기준으로 "이 책임이 이 클래스에 있어도 되는가?"를 계속 확인하면 구조가 훨씬 안정적으로 잡힌다.

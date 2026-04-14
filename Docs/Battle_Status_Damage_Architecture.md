# ArrowHunter 전투 / 데미지 / 상태이상 구조 정리

이 문서는 `DamageContext`, `DamageResolver`, `DirectionInputResult` 기반 리팩토링이 적용되었다는 가정으로 작성했다.
아이템과 보상 선택 시스템을 설계하기 전에 전투 코드의 역할, 흐름, 함수 사용 기준을 복습하기 위한 문서다.

## 1. 큰 설계 기준

전투 시스템은 크게 네 축으로 나눈다.

```text
BattleActionService
-> 전투 입력과 턴 진행을 처리한다.

DamageResolver
-> 모든 데미지 계산을 담당한다.

StatusHandler
-> 현재 걸린 상태이상 목록을 관리하고, 상태이상 훅을 호출한다.

StatusEffectSO
-> 개별 상태이상의 실제 효과를 정의한다.
```

핵심 규칙은 다음과 같다.

```text
데미지에 영향을 주는 효과
-> DamageContext를 수정한다.

입력에 영향을 주는 효과
-> DirectionInputResult를 수정한다.

턴 시작 / 턴 종료 / 적용 / 해제 효과
-> StatusEffectSO의 생명주기 훅에서 처리한다.

최종 HP 감소
-> BattleEntity.TakeDamage()에서 처리한다.
```

## 2. 파일별 역할

### BattleActionService

역할:

- 플레이어 방향 입력을 받는다.
- 스킬 입력 시작/종료를 처리한다.
- 코스트 소비를 처리한다.
- 공격 코루틴을 실행한다.
- 데미지 계산 자체는 직접 하지 않고 `DamageResolver`에 맡긴다.

주요 흐름:

```text
TryPlayerAttack(rawDir)
-> player.statusHandler.ModifyDirectionInput(rawDir)
-> 실패면 코스트 소비 / 스킬 버퍼 취소 / 턴 종료 여부 처리
-> 성공이면 SkillExecutor.OnDirectionInput(direction)
-> 코스트 소비
-> DamageResolver.CreateBasicAttack(...)
-> DamageResolver.Apply(...)
```

주의:

- 마비나 동결처럼 입력을 막는 상태이상은 `DirectionInputResult`를 통해 처리한다.
- `Direction.None`만으로 실패 이유를 표현하지 않는다.
- 방향 입력 실패와 턴 종료는 별개다.

## 3. BattleEntity

역할:

- 전투 중 한 개체의 HP와 기본 스탯 접근을 담당한다.
- 플레이어면 `PlayerInstance` 값을 읽고, 적이면 `BaseStatSO` 값을 읽는다.
- 최종 피해를 받아 HP만 감소시킨다.

권장 책임:

```text
GetAttackPower(direction)
GetAverageAttackPower()
GetSkillPower()
GetStatusPowerValue()
GetDefensePower()
GetComboDamageMultiplier()
GetDamageTakenMultiplier()
TakeDamage(finalDamage)
```

중요한 기준:

```text
BattleEntity.TakeDamage()
-> 이미 계산이 끝난 최종 피해만 받는다.
-> 상태이상 데미지 수정, 방어력 계산, 취약 계산은 여기서 하지 않는다.
```

즉 `TakeDamage()`는 이런 역할만 가진다.

```csharp
public void TakeDamage(int finalDamage)
{
    currentHp -= Mathf.Max(0, finalDamage);
    currentHp = Mathf.Max(currentHp, 0);

    if (currentHp <= 0)
        OnDie();
}
```

## 4. DamageContext

`DamageContext`는 이번 피해 계산에 필요한 정보를 담는 객체다.

예상 필드:

```csharp
public class DamageContext
{
    public BattleEntity source;
    public BattleEntity target;

    public DamageType damageType;
    public StatusType? statusType;

    public Direction direction;
    public bool isCombo;

    public float damage;
    public float defensePower;
    public DamagePhase phase;

    public bool useDefense = true;
    public bool useOutgoingModifiers = true;
    public bool useIncomingModifiers = true;
    public bool useDamageTakenMultiplier = true;
}
```

`DamageType` 예시:

```text
BasicAttack
Skill
Status
TrueDamage
```

`DamagePhase` 예시:

```text
BeforeDefense
Defense
AfterDefense
Final
```

왜 필요한가:

- 기본 공격, 스킬, 독, 화상, 고정 피해를 구분할 수 있다.
- 취약이 독 피해에도 적용될지, 기본 공격에만 적용될지 판단할 수 있다.
- 방어력 감소와 최종 피해 증가를 분리할 수 있다.

## 5. DamageResolver

`DamageResolver`는 데미지 계산의 중심이다.

역할:

- 공격력, 콤보 배율, 방어력, 상태이상 수정, 최종 피해 배율을 순서대로 적용한다.
- 계산된 최종 피해를 `BattleEntity.TakeDamage()`로 넘긴다.

권장 흐름:

```text
Resolve(context)
-> BeforeDefense 단계
-> source.statusHandler.ModifyDamage(context)
-> Defense 단계
-> target.GetDefensePower()
-> target.statusHandler.ModifyDamage(context)
-> 방어력 적용
-> AfterDefense 단계
-> target.statusHandler.ModifyDamage(context)
-> Final 단계
-> DamageTakenMultiplier 적용
-> 최종 int 피해 반환
```

기본 공격 생성 예:

```csharp
DamageContext context = DamageResolver.CreateBasicAttack(
    source,
    target,
    direction,
    isCombo
);

DamageResolver.Apply(context);
```

상태이상 피해 생성 예:

```csharp
DamageContext context = DamageResolver.CreateStatusDamage(
    source,
    target,
    StatusType.Burn,
    damage
);

DamageResolver.Apply(context);
```

## 6. StatusEffectSO

`StatusEffectSO`는 상태이상 설계도다.

공통 설정:

```text
effectName
duration
maxStack
durationRule
stackRule
isActionDisable
disablesSkill
```

생명주기 훅:

```csharp
OnApply(owner, attacker, instance)
OnRemove(owner, instance)
OnTurnStart(owner, instance)
OnTurnEnd(owner, instance)
OnDirectionInput(owner, instance)
OnStack(instance)
```

입력 수정 훅:

```csharp
ModifyDirectionInput(owner, instance, input)
```

데미지 수정 훅:

```csharp
ModifyDamage(owner, instance, context)
```

중요한 기준:

```text
개별 상태이상은 StatusHandler를 직접 알 필요가 없다.
자기 효과가 필요한 훅만 override한다.
```

## 7. StatusEffectInstance

`StatusEffectInstance`는 전투 중 실제로 걸려 있는 상태이상 1개다.

예상 필드:

```csharp
public StatusEffectSO data;
public BattleEntity source;

public int remainingDuration;
public int currentStack;

public float magnitude;
public int intValue;
public Direction storedDirection;
```

필드 용도:

```text
data
-> 어떤 상태이상 SO인지 가리킨다.

source
-> 누가 이 상태이상을 걸었는지 저장한다.

remainingDuration
-> 남은 턴 수다.

currentStack
-> 현재 중첩 수다.

magnitude
-> 화상 피해량, 취약 비율, 출혈 피해량 같은 실수형 값을 저장한다.

intValue
-> 특수 카운트나 정수형 값을 저장할 때 쓴다.

storedDirection
-> 동결처럼 특정 방향을 저장할 때 쓴다.
```

## 8. StatusHandler

`StatusHandler`는 `BattleEntity`가 가진 상태이상 관리자다.

핵심 필드:

```csharp
private BattleEntity _owner;
private List<StatusEffectInstance> _activeEffects;
```

주요 함수:

```text
Apply()
-> 상태이상을 새로 적용하거나 기존 상태이상을 갱신한다.

Tick()
-> 턴 종료 효과를 실행하고 지속시간을 줄인다.

OnTurnStart()
-> 턴 시작 효과를 실행한다.

OnDirectionInput()
-> 방향 입력 시 반응하는 상태이상을 실행한다.

ModifyDirectionInput()
-> 동결, 마비처럼 입력 자체를 바꾸거나 실패시킨다.

ModifyDamage()
-> 데미지 계산 중 상태이상들이 DamageContext를 수정하게 한다.

IsActionDisabled()
-> 기절처럼 행동 불가 상태인지 확인한다.

IsSkillDisabled()
-> 스킬 사용 불가 상태인지 확인한다.

GetActiveEffectString()
-> UI 표시용 상태이상 문자열을 만든다.
```

`StatusHandler`가 하면 안 되는 일:

```text
독이면 몇 데미지를 준다.
화상이면 방어력을 얼마나 깎는다.
마비면 몇 퍼센트로 실패한다.
취약이면 피해를 얼마나 늘린다.
```

이런 세부 규칙은 전부 개별 `StatusEffectSO`가 담당한다.

## 9. DirectionInputResult

`DirectionInputResult`는 방향 입력의 최종 결과를 표현한다.

필드:

```csharp
public Direction direction;
public bool isFailed;
public bool spendCostOnFail;
public bool cancelSkillBuffer;
public bool endTurnOnFail;
public string failReason;
```

사용 이유:

```text
동결
-> 특정 방향 입력 실패
-> 코스트 소비 없음
-> 스킬 버퍼 유지
-> 턴 유지

마비
-> 확률적 입력 실패
-> 코스트 소비
-> 스킬 버퍼 취소
-> 턴 유지

기절
-> 턴 시작 시 행동 불가
-> DirectionInputResult가 아니라 isActionDisable로 처리
```

즉 입력 실패의 종류를 `Direction.None` 하나로 표현하지 않는다.

## 10. 상태이상별 권장 매핑

### Poison

역할:

```text
턴 종료 시 최대 체력 비례 피해
```

사용 훅:

```text
OnApply
OnTurnEnd
OnStack
```

권장 흐름:

```text
OnApply
-> instance.magnitude에 기본 비율 저장

OnTurnEnd
-> 대상 최대 체력 * magnitude로 상태이상 피해 생성
-> DamageResolver.CreateStatusDamage(...)
-> DamageResolver.Apply(...)

OnStack
-> magnitude 증가
```

### Burn

역할:

```text
턴 종료 피해
방어력 약화
```

사용 훅:

```text
OnApply
OnTurnEnd
ModifyDamage
```

권장 흐름:

```text
OnApply
-> instance.magnitude에 화상 피해량 저장

OnTurnEnd
-> DamageType.Status / StatusType.Burn 피해 적용

ModifyDamage
-> context.phase == Defense
-> context.target == owner
-> context.defensePower *= 1f - defenseReductionRatio
```

### Bleed

역할:

```text
방향 입력 시 피해
```

사용 훅:

```text
OnApply
OnDirectionInput
OnStack
```

권장 흐름:

```text
OnApply
-> instance.magnitude에 출혈 피해량 저장

OnDirectionInput
-> DamageType.Status / StatusType.Bleed 피해 적용

OnStack
-> magnitude 또는 currentStack 증가
```

### Freeze

역할:

```text
특정 방향 봉인
```

사용 훅:

```text
OnApply
ModifyDirectionInput
```

권장 흐름:

```text
OnApply
-> instance.storedDirection에 봉인 방향 저장

ModifyDirectionInput
-> 입력 방향이 storedDirection이면 DirectionInputResult.Fail(...)
-> 코스트 소비 없음
-> 스킬 버퍼 취소 없음
-> 턴 종료 없음
```

### Stun

역할:

```text
행동 불가
```

사용 방식:

```text
StatusEffectSO 인스펙터에서 isActionDisable = true
필요하면 disablesSkill = true
```

권장 코드:

```text
별도 로직이 거의 필요 없다.
OnApply는 로그 정도만 사용한다.
duration-- 같은 SO 값 변경은 하지 않는다.
```

### Paralyze

역할:

```text
일정 확률로 입력 실패
실패 시 코스트 소비
실패 시 스킬 버퍼 취소
턴은 넘어가지 않음
```

사용 훅:

```text
ModifyDirectionInput
```

권장 흐름:

```text
Random.value로 실패 여부 계산
실패하면 DirectionInputResult.Fail(
    "Paralyze",
    spendCost: true,
    cancelSkillBuffer: true,
    endTurn: false
)
```

### Vulnerable

역할:

```text
받는 최종 피해 증가
```

사용 훅:

```text
ModifyDamage
```

권장 흐름:

```text
context.phase == AfterDefense
context.target == owner
context.damage *= 1f + takenDamageIncreaseRatio
```

### Weak

역할:

```text
주는 피해 감소
```

사용 훅:

```text
ModifyDamage
```

권장 흐름:

```text
context.phase == BeforeDefense
context.source == owner
context.damage *= 1f - outgoingDamagePenalty
```

### Exhaustion

역할:

```text
코스트 회복 감소
```

사용 훅:

```text
ModifyCostRecovery
```

권장 흐름:

```text
회복량 *= 1f - recoveryPenalty
```

### Distraction

역할:

```text
커맨드 입력 개수 제한
```

사용 훅:

```text
ModifyCommandInputLimit
```

현재 없다면 나중에 추가할 훅:

```csharp
public virtual int ModifyCommandInputLimit(
    BattleEntity owner,
    StatusEffectInstance instance,
    int limit)
{
    return limit;
}
```

## 11. 기본 공격 데미지 흐름

```text
플레이어 방향 입력
-> BattleActionService.TryPlayerAttack(rawDir)
-> player.statusHandler.ModifyDirectionInput(rawDir)
-> 실패면 DirectionInputResult 설정에 따라 처리
-> 성공하면 코스트 소비
-> DamageResolver.CreateBasicAttack(player, enemy, direction, isCombo)
-> DamageResolver.Apply(context)
-> DamageResolver.Resolve(context)
-> 공격자 상태이상 ModifyDamage
-> 방어자 상태이상 ModifyDamage
-> 방어력 적용
-> 방어자 상태이상 ModifyDamage
-> 최종 피해 계산
-> target.TakeDamage(finalDamage)
```

## 12. 상태이상 피해 흐름

예: 화상

```text
enemy.statusHandler.Tick()
-> BurnEffectSO.OnTurnEnd(owner, instance)
-> DamageResolver.CreateStatusDamage(instance.source, owner, StatusType.Burn, instance.magnitude)
-> DamageResolver.Apply(context)
-> DamageType.Status로 계산
-> target.TakeDamage(finalDamage)
```

주의:

```text
상태이상 피해를 owner.TakeDamage()로 직접 호출하면
DamageResolver의 규칙을 우회하게 된다.

가능하면 상태이상 피해도 DamageResolver를 통과시킨다.
```

## 13. 보상 / 아이템 시스템과 연결할 때

보상은 가능하면 `StatModifier`로 저장한다.

예:

```text
위 공격력 +3
-> StatType.AttackUp / Flat / +3

콤보 피해 +15%
-> StatType.ComboDamageMultiplier / Percent / +0.15

화상 효과 +20%
-> StatType.BurnPowerMultiplier / Percent / +0.2

상태이상 피해 +10%
-> StatType.StatusDamageMultiplier / Percent / +0.1
```

기준:

```text
캐릭터 기본 성향
-> BaseStatSO

플레이어가 획득한 보상 / 아이템 / 패시브
-> PlayerInstance의 StatModifier

전투 중 임시 효과
-> StatusEffectSO 또는 임시 StatModifier
```

## 14. 새 상태이상 추가 체크리스트

새 상태이상을 만들 때 먼저 질문한다.

```text
1. 이 효과는 턴 시작/종료에 발동하는가?
-> OnTurnStart / OnTurnEnd

2. 이 효과는 방향 입력을 막거나 바꾸는가?
-> ModifyDirectionInput

3. 이 효과는 방향 입력 후 반응하는가?
-> OnDirectionInput

4. 이 효과는 데미지 계산에 영향을 주는가?
-> ModifyDamage

5. 이 효과는 코스트 회복에 영향을 주는가?
-> ModifyCostRecovery

6. 이 효과는 단순 행동 불가인가?
-> isActionDisable

7. 이 효과는 스킬 사용을 막는가?
-> disablesSkill
```

이 질문에 답하면 어느 함수에 구현해야 하는지 정해진다.

## 15. 정리해야 할 가능성이 있는 부분

리팩토링 후 확인할 부분:

```text
FilterDirection / IDirectionFilter
-> 구형 구조다.
-> ModifyDirectionInput으로 통일하면 제거 가능하다.

ModifyIncomingDamage / ModifyOutgoingDamage / ModifyDefensePower
-> DamageContext 기반 ModifyDamage로 통합 가능하다.

StatusEffectData
-> 현재 사용하지 않는 구형 데이터 클래스라면 제거 후보.

BattleEntity.TakeDamage
-> 상태이상 계산 없이 최종 피해만 적용하는지 확인.

CombatProcessor
-> DamageResolver로 대체되면 제거하거나 얇은 래퍼로 변경.

EnemyTurnState
-> 방어 입력에도 동결/마비가 적용되어야 하면 ModifyDirectionInput을 거치게 변경.

StatusType
-> Vulnerable, Weak, Exhaustion, Distraction이 enum에 추가되어 있는지 확인.
```

## 16. 한 줄 요약

```text
전투 입력은 BattleActionService,
피해 계산은 DamageResolver,
상태이상 목록 관리는 StatusHandler,
상태이상 개별 효과는 StatusEffectSO,
최종 HP 감소는 BattleEntity가 담당한다.
```

가장 중요한 기준:

```text
데미지 효과는 DamageContext를 수정한다.
입력 방해 효과는 DirectionInputResult를 수정한다.
상태이상 이름별 분기문은 StatusHandler에 넣지 않는다.
```

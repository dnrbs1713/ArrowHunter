using UnityEngine;
using ArrowClash.Common;
[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "Scriptable Objects/StatusEffectSO")]
public class StatusEffectSO : ScriptableObject
{
    [Header("기본 정보")]
    public StatusType effectName;
    public int duration;
    public int maxStack;

    [Header("턴 / 중첩 규칙")]
    public DurationRule durationRule;
    public StackRule stackRule;

    [Header("행동불능")]
    public bool isActionDisable;
    public bool disablesSkill;

    public virtual void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance) { }
    public virtual void OnTurnEnd(BattleEntity owner, StatusEffectInstance instance) { }
    public virtual void OnDirectionInput(BattleEntity owner, StatusEffectInstance instance) { }
    public virtual void OnStack(StatusEffectInstance instance) { }
}

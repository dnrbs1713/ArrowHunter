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
    public virtual void OnRemove(BattleEntity owner, StatusEffectInstance instance) { }

    public virtual void OnTurnStart(BattleEntity owner, StatusEffectInstance instance) { }
    public virtual void OnTurnEnd(BattleEntity owner, StatusEffectInstance instance) { }

    public virtual void OnDirectionInput(BattleEntity owner, StatusEffectInstance instance) { }
    public virtual void OnStack(StatusEffectInstance instance) { }

    public virtual int ModifyIncomingDamage(BattleEntity owner, StatusEffectInstance instance, int damage)
    {
        return damage;
    }

    public virtual int ModifyOutgoingDamage(BattleEntity owner, StatusEffectInstance instance, int damage)
    {
        return damage;
    }

    public virtual int ModifyCostRecovery(BattleEntity owner, StatusEffectInstance instance, int value)
    {
        return value;
    }
    public virtual float ModifyDefensePower(BattleEntity owner, StatusEffectInstance instance, float defensePower)
    {
        return defensePower;
    }

    public virtual DirectionInputResult ModifyDirectionInput(BattleEntity owner, StatusEffectInstance instance, 
        DirectionInputResult input)
    {
        return input;
    }

    public virtual void ModifyDamage(BattleEntity owner,StatusEffectInstance instance,DamageContext context)
    {
    }

}

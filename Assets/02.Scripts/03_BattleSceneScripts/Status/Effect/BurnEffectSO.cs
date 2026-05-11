using UnityEngine;

[CreateAssetMenu(fileName = "BurnEffectSO", menuName = "Scriptable Objects/BurnEffectSO")]
public class BurnEffectSO : StatusEffectSO
{
    [Header("화상 피해 계수")]
    public float damageMultiplier = 1.2f;

    [Header("방어력 감소율")]
    [Range(0f, 1f)]
    public float defenseReductionRatio = 0.2f;
    public override void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance)
    {
        instance.magnitude = attacker.GetStatusPowerValue() * damageMultiplier;
    }

    public override void OnTurnEnd(BattleEntity owner, StatusEffectInstance instance)
    {
        DamageContext context = DamageResolver.CreateStatusDamage(
            instance.source,
            owner,
            StatusType.Burn,
            instance.magnitude
        );

        DamageResolver.Apply(context);
    }

    public override void ModifyDamage(BattleEntity owner, StatusEffectInstance instance, DamageContext context)
    {
        if (context.phase != DamagePhase.Defense) return;
        if (context.target != owner) return;

        context.defensePower *= 1f - defenseReductionRatio;
    }
}

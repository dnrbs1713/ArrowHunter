using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class BleedEffectSO : StatusEffectSO
{
    [Header("출혈 수치")]
    public float attackRatio = 0.15f;

    public override void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance)
    {
        instance.magnitude = attacker.GetStatusPowerValue() * attackRatio;
    }

    public override void OnDirectionInput(BattleEntity owner, StatusEffectInstance instance)
    {
        float damage = instance.magnitude;

        DamageContext context = DamageResolver.CreateStatusDamage(
            instance.source,
            owner,
            StatusType.Bleed,
            damage,
            useIncomingModifiers: true,
            useDefense : true
        );

        int finalDamage = DamageResolver.Apply(context);

        Debug.Log($"<color=red>[출혈] {finalDamage} 데미지 / 스택: {instance.currentStack}</color>");
    }
    public override void OnStack(StatusEffectInstance instance)
    {
        if (instance.currentStack >= instance.data.maxStack)
            return;

        instance.magnitude += instance.magnitude / instance.currentStack;
        instance.currentStack++;
    }
}

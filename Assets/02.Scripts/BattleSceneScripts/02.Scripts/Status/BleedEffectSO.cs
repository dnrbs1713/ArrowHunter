using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class BleedEffectSO : StatusEffectSO
{
    [Header("출혈 수치")]
    public float attackRatio = 0.15f;

    public override void OnApply(BattleEntity owner, BattleEntity attacker,StatusEffectInstance instance)
    {
        instance.currentDamageValue = attacker.statData.attackDamage * attackRatio;
    }
    public override void OnDirectionInput(BattleEntity owner, StatusEffectInstance instance)
    {
        int damage = Mathf.RoundToInt(instance.currentDamageValue);
        owner.TakeDamage(damage);
        Debug.Log($"<color=red>[출혈] {damage} 데미지 / 스택: {instance.currentStack}</color>");
    }
    public override void OnStack(StatusEffectInstance instance)
    {
        instance.currentDamageValue += instance.currentDamageValue / instance.currentStack;
        instance.currentStack++;
    }
}

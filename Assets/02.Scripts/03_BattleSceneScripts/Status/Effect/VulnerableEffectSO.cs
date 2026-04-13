using UnityEngine;

[CreateAssetMenu(fileName = "VulnerableEffectSO", menuName = "Scriptable Objects/VulnerableEffectSO")]
public class VulnerableEffectSO : StatusEffectSO
{
    [Header("피해 증가량")]
    [Range(0f, 1f)]
    public float takenDamageIncreaseRatio = 0.3f;

    public override void ModifyDamage(BattleEntity owner, StatusEffectInstance instance, DamageContext context)
    {
        if (context.phase != DamagePhase.AfterDefense) return;
        if (context.target != owner) return;
        if (!context.useIncomingModifiers) return;

        context.damage *= 1f + takenDamageIncreaseRatio;
    }
}

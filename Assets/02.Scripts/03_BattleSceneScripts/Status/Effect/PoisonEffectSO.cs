using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffectSO", menuName = "Scriptable Objects/PoisonEffectSO")]
public class PoisonEffectSO : StatusEffectSO
{
    [Header("독 수치")]
    public float damageRatio = 0.05f;
    public float stackMultiplier = 1.03f;
    public float maxRatio = 0.10f;

    public override void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance)
    {
        instance.magnitude = damageRatio;
    }
    public override void OnTurnEnd(BattleEntity owner, StatusEffectInstance instance)
    {
        float ratio = Mathf.Min(instance.magnitude, maxRatio);
        float damage = owner.GetMaxHp() * ratio;
        DamageContext context = DamageResolver.CreateStatusDamage(
            instance.source,
            owner,
            StatusType.Poison,
            damage
        );

        int finalDamage = DamageResolver.Apply(context);

        Debug.Log($"<color=green>[독] {finalDamage} 데미지 / 비율: {ratio:P0}</color>");

    }
    public override void OnStack(StatusEffectInstance instance)
    {
        instance.magnitude = Mathf.Min(instance.magnitude * stackMultiplier, maxRatio);
        instance.currentStack++;
    }
}

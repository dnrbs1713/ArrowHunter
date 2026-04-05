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
        instance.currentDamageValue = damageRatio; ;
    }
    public override void OnTurnEnd(BattleEntity owner, StatusEffectInstance instance)
    {
        float ratio = Mathf.Min(instance.currentDamageValue, maxRatio);
        int damage = Mathf.RoundToInt(owner.statData.maxHp * ratio);
        owner.TakeDamage(damage);
        Debug.Log($"<color=green>[독] {damage} 데미지 / 비율: {ratio:P0}</color>");
    }
    public override void OnStack(StatusEffectInstance instance)
    {
        instance.currentDamageValue *= stackMultiplier;
        instance.currentStack++;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "BurnEffectSO", menuName = "Scriptable Objects/BurnEffectSO")]
public class BurnEffectSO : StatusEffectSO
{
    [Header("화상 수치")]
    public float attackMultiplier = 1.2f;

    public override void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance)
    {
        instance.currentDamageValue = attacker.statData.attackDamage * attackMultiplier;
    }
    public override void OnTurnEnd(BattleEntity owner, StatusEffectInstance instance)
    {
        int damage = Mathf.RoundToInt(instance.currentDamageValue);
        owner.TakeDamage(damage);
        Debug.Log($"<color=orange>[화상] {damage} 데미지 / 남은 턴: {instance.remainingDuration}</color>");
    }
}

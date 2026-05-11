using UnityEngine;

[CreateAssetMenu(fileName = "ChanceDamageBonusItemEffectSO", menuName = "Scriptable Objects/ChanceDamageBonusItemEffectSO")]
public class ChanceDamageBonusItemEffectSO : ItemEffectSO
{
    [Range(0f, 1f)]
    public float chance = 1.0f;

    [Range(0f, 1f)]
    public float damageBonusPercent = 0.5f;

    public bool basicAttackOnly = true;

    public override void HandleBattleEvent(ItemBattleEventContext context)
    {
        if (context.eventType != ItemBattleEventType.BeforeDealDamage)
            return;

        if (context.damageContext == null)
            return;

        if (context.damageContext.damageType != DamageType.BasicAttack)
            return;

        if (Random.value >= chance)
            return;

        context.damageContext.damage *= 1f + damageBonusPercent;

        Debug.Log("이끼 낀 돌검 데미지 보너스!");
    }
}

using NUnit.Framework.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/DamageEffect")]
public class DamageEffect : SkillEffectSO
{
    [Header("데미지")]
    public float damageMultiplier = 1.5f;
    public override void Apply(BattleEntity caster, BattleEntity target)
    {
        int damage = Mathf.Max(1, Mathf.RoundToInt(caster.statData.attackDamage * damageMultiplier
            * (1f - target.statData.defensePower)));
        target.TakeDamage(damage);
        Debug.Log($"<color=lime>[스킬 데미지] {damage} ({damageMultiplier}배)</color>");
    }
}

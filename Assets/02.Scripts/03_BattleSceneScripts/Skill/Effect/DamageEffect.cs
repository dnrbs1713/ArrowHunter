using NUnit.Framework.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/DamageEffect")]
public class DamageEffect : SkillEffectSO
{
    [Header("데미지")]
    public float damageMultiplier = 1.5f;

    [Header("계산 옵션")]
    public bool useDefense = true;
    public bool useOutgoingModifiers = true;
    public bool useIncomingModifiers = true;
    public bool useDamageTakenMultiplier = true;
    public override void Apply(BattleEntity caster, BattleEntity target)
    {
        DamageContext context = DamageResolver.CreateSkillDamage(
            caster,
            target,
            damageMultiplier,
            useDefense,
            useOutgoingModifiers,
            useIncomingModifiers,
            useDamageTakenMultiplier
        );

        int damage = DamageResolver.Apply(context);

        Debug.Log($"<color=lime>[스킬 데미지] {damage} ({damageMultiplier}배)</color>");
    }
}

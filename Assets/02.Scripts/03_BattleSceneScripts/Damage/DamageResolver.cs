using UnityEngine;
using ArrowClash.Common;

public static class DamageResolver
{
    private const float DefenseScaling = 100f;
    public static int Resolve(DamageContext context)
    {
        context.phase = DamagePhase.BeforeDefense;

        if (context.useOutgoingModifiers && context.source != null)
            context.source.statusHandler.ModifyDamage(context);

        if (context.useDefense)
        {
            context.defensePower = context.target.GetDefensePower();

            context.phase = DamagePhase.Defense;
            context.target.statusHandler.ModifyDamage(context);

            float reduction = GetDefenseReduction(context.defensePower);
            context.damage *= 1f - reduction;
        }

        context.phase = DamagePhase.AfterDefense;

        if (context.useIncomingModifiers)
            context.target.statusHandler.ModifyDamage(context);

        context.phase = DamagePhase.Final;

        if (context.useDamageTakenMultiplier)
            context.damage *= context.target.GetDamageTakenMultiplier();

        return Mathf.Max(1, Mathf.RoundToInt(context.damage));
    }

    public static int Apply(DamageContext context)
    {
        int finalDamage = Resolve(context);
        context.target.TakeDamage(finalDamage);
        return finalDamage;
    }

    // 방어력 수치 -> 피해 감소율
    private static float GetDefenseReduction(float defensePower)
    {
        defensePower = Mathf.Max(0f, defensePower);
        return defensePower / (defensePower + DefenseScaling);
    }

    public static DamageContext CreateBasicAttack(
        BattleEntity source,
        BattleEntity target,
        Direction direction,
        bool isCombo)
    {
        float damage = source.GetAttackPower(direction);

        if (isCombo)
            damage *= source.GetComboDamageMultiplier();

        return new DamageContext
        {
            source = source,
            target = target,
            damageType = DamageType.BasicAttack,
            direction = direction,
            isCombo = isCombo,
            damage = damage,
            useDefense = true,
            useOutgoingModifiers = true,
            useIncomingModifiers = true,
            useDamageTakenMultiplier = true
        };
    }

    public static DamageContext CreateSkillDamage(
        BattleEntity source,
        BattleEntity target,
        float damageMultiplier,
        bool useDefnese = true,
        bool useOutgoingModifiers = true,
        bool useIncomingModifiers = true,
        bool useDamageTakenMultiplier = true)
    {
        float damage = source.GetSkillPower() * damageMultiplier;

        return new DamageContext
        {
            source = source,
            target = target,
            damageType = DamageType.Skill,
            damage = damage,

            useDefense = useDefnese,
            useOutgoingModifiers = useOutgoingModifiers,
            useIncomingModifiers = useIncomingModifiers,
            useDamageTakenMultiplier = useDamageTakenMultiplier
        };
    }

    public static DamageContext CreateStatusDamage(
        BattleEntity source,
        BattleEntity target,
        StatusType statusType,
        float damage,
        bool useIncomingModifiers = false,
        bool useDefense = false
        )
    {
        return new DamageContext
        {
            source = source,
            target = target,
            damageType = DamageType.Status,
            statusType = statusType,
            damage = damage,
            useDefense = false,
            useOutgoingModifiers = false,
            useIncomingModifiers = false,
            useDamageTakenMultiplier = true
        };
    }
}

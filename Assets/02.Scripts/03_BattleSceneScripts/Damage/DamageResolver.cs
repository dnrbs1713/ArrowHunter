using UnityEngine;
using ArrowClash.Common;

public static class DamageResolver
{
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

            context.defensePower = Mathf.Clamp01(context.defensePower);
            context.damage *= 1f - context.defensePower;
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

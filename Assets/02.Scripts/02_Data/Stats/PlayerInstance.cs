using System.Collections.Generic;
using UnityEngine;
using ArrowClash.Common;

public class PlayerInstance
{
    public BaseStatSO baseStat { get; private set; }
    public int currentHp { get; private set; }

    private readonly List<StatModifier> _modifiers = new List<StatModifier>();
    public IReadOnlyList<StatModifier> Modifiers => _modifiers;

    public PlayerInstance(BaseStatSO so)
    {
        baseStat = so;
        currentHp = so.maxHp;
    }

    public int MaxHp => Mathf.RoundToInt(GetStat(StatType.MaxHp));
    public int MaxCost => Mathf.RoundToInt(GetStat(StatType.MaxCost));
    public int StartCost => Mathf.RoundToInt(GetStat(StatType.StartCost));
    public int CostRecovery => Mathf.RoundToInt(GetStat(StatType.CostRecovery));

    public float DefensePower => Mathf.Clamp01(GetStat(StatType.DefensePower));
    public float StatusPower => Mathf.Max(0f, GetStat(StatType.StatusPower));
    public float StatusResistance => Mathf.Clamp01(GetStat(StatType.StatusResistance));

    public float BasicAttackMultiplier => Mathf.Max(0f, GetStat(StatType.BasicAttackMultiplier));
    public float SkillPowerMultiplier => Mathf.Max(0f, GetStat(StatType.SkillPowerMultiplier));
    public float DamageTakenMultiplier => Mathf.Max(0f, GetStat(StatType.DamageTakenMultiplier));
    public float ComboDamageMultiplier => Mathf.Max(0f, GetStat(StatType.ComboDamageMultiplier));

    public int GetAttack(Direction dir)
    {
        StatType attackType = GetAttackStatType(dir);
        float finalAttack = GetStat(attackType) * BasicAttackMultiplier;
        return Mathf.Max(1, Mathf.RoundToInt(finalAttack));
    }

    public int GetAverageAttack()
    {
        float total =
            GetStat(StatType.AttackUp) +
            GetStat(StatType.AttackDown) +
            GetStat(StatType.AttackLeft) +
            GetStat(StatType.AttackRight);

        return Mathf.Max(1, Mathf.RoundToInt(total / 4f));
    }

    public int GetSkillPowerValue()
    {
        float value = GetAverageAttack() * SkillPowerMultiplier;
        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public int GetStatusPowerValue()
    {
        float value = GetAverageAttack() * StatusPower;
        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public float GetStat(StatType type)
    {
        float baseValue = GetBaseValue(type);
        float flat = 0f;
        float percent = 0f;

        for (int i = 0; i < _modifiers.Count; i++)
        {
            StatModifier mod = _modifiers[i];
            if (mod.statType != type) continue;

            switch (mod.mode)
            {
                case ModifierMode.Flat:
                    flat += mod.value;
                    break;

                case ModifierMode.Percent:
                    percent += mod.value;
                    break;
            }
        }

        return (baseValue + flat) * (1f + percent);
    }

    private float GetBaseValue(StatType type)
    {
        return type switch
        {
            StatType.MaxHp => baseStat.maxHp,

            StatType.AttackUp => baseStat.attackPower.up,
            StatType.AttackDown => baseStat.attackPower.down,
            StatType.AttackLeft => baseStat.attackPower.left,
            StatType.AttackRight => baseStat.attackPower.right,

            StatType.DefensePower => baseStat.defensePower,

            StatType.MaxCost => baseStat.maxCost,
            StatType.StartCost => baseStat.startCost,
            StatType.CostRecovery => baseStat.baseCostRecovery,

            StatType.StatusPower => baseStat.statusPower,
            StatType.StatusResistance => baseStat.statusResistance,

            StatType.BasicAttackMultiplier => baseStat.basicAttackMultiplier,
            StatType.SkillPowerMultiplier => baseStat.skillPowerMultiplier,
            StatType.ComboDamageMultiplier => baseStat.comboDamageMultiplier,
            StatType.DamageTakenMultiplier => baseStat.damageTakenMultiplier,

            StatType.BurnPowerMultiplier => 1f,
            StatType.PoisonPowerMultiplier => 1f,
            StatType.BleedPowerMultiplier => 1f,
            StatType.StatusDamageMultiplier => 1f,
            StatType.StatusDurationBonus => 0f,


            _ => 0f
        };
    }

    private StatType GetAttackStatType(Direction dir)
    {
        return dir switch
        {
            Direction.Up => StatType.AttackUp,
            Direction.Down => StatType.AttackDown,
            Direction.Left => StatType.AttackLeft,
            Direction.Right => StatType.AttackRight,
            _ => StatType.AttackUp
        };
    }

    public void AddModifier(StatModifier modifier)
    {
        _modifiers.Add(modifier);
        ClampCurrentHp();
    }

    public void AddModifiers(IEnumerable<StatModifier> modifiers)
    {
        foreach (var modifier in modifiers)
            _modifiers.Add(modifier);

        ClampCurrentHp();
    }

    public void RemoveModifiersBySource(string sourceId)
    {
        _modifiers.RemoveAll(m => m.sourceId == sourceId);
        ClampCurrentHp();
    }

    public void TickTemporaryModifiers()
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].IsPermanent) continue;

            _modifiers[i].Tick();

            if (_modifiers[i].IsExpired)
                _modifiers.RemoveAt(i);
        }

        ClampCurrentHp();
    }

    public void SetCurrentHp(int value)
    {
        currentHp = Mathf.Clamp(value, 0, MaxHp);
    }

    public void RestoreFullHp()
    {
        currentHp = MaxHp;
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = Mathf.Max(0, Mathf.RoundToInt(amount * DamageTakenMultiplier));
        currentHp = Mathf.Clamp(currentHp - finalDamage, 0, MaxHp);
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Clamp(currentHp + amount, 0, MaxHp);
    }

    private void ClampCurrentHp()
    {
        currentHp = Mathf.Clamp(currentHp, 0, MaxHp);
    }
}

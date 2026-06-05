using System.Collections.Generic;
using UnityEngine;
using ArrowClash.Common;
using Unity.VisualScripting.Dependencies.Sqlite;
using TMPro;

public class PlayerInstance
{
    public BaseStatSO baseStat { get; private set; }
    public int currentHp { get; private set; }
    public int DefenseSuccessCostBonus => Mathf.RoundToInt(GetStat(StatType.DefenseSuccessCostBonus));

    private readonly List<StatModifier> _modifiers = new List<StatModifier>();
    public IReadOnlyList<StatModifier> Modifiers => _modifiers;

    public int level { get; private set; } = 1;

    public int Level => level;
    public int currentExp { get; private set; } = 0;

    private readonly List<PlayerSkillInstance> _skills = new List<PlayerSkillInstance>();
    public IReadOnlyList<PlayerSkillInstance> Skills => _skills;

    public int skillPoints { get; private set; }

    [SerializeField]
    public PlayerInventory playerInventory { get; private set; }
    public EquipmentSlot equipmentSlot { get; private set; }
    public PlayerArtifactCollection playerArtifactCollection { get; private set; }

    public PlayerInstance(BaseStatSO so)
    {
        baseStat = so;
        currentHp = so.maxHp;

        playerInventory = new PlayerInventory(this);
        equipmentSlot = new EquipmentSlot(this);
        playerArtifactCollection = new PlayerArtifactCollection(this);
    }

    public int MaxHp => Mathf.RoundToInt(GetStat(StatType.MaxHp));
    public int MaxCost => Mathf.RoundToInt(GetStat(StatType.MaxCost));
    public int StartCost => Mathf.RoundToInt(GetStat(StatType.StartCost));
    public int CostRecovery => Mathf.RoundToInt(GetStat(StatType.CostRecovery));

    public float DefensePower => Mathf.Max(0f, GetStat(StatType.DefensePower));
    public float StatusPower => Mathf.Max(0f, GetStat(StatType.StatusPower));
    public float StatusResistance => Mathf.Clamp01(GetStat(StatType.StatusResistance));

    public float BasicAttackMultiplier => Mathf.Max(0f, GetStat(StatType.BasicAttackMultiplier));
    public float SkillPowerMultiplier => Mathf.Max(0f, GetStat(StatType.SkillPowerMultiplier));
    public float DamageTakenMultiplier => Mathf.Max(0f, GetStat(StatType.DamageTakenMultiplier));
    public float ComboDamageMultiplier => Mathf.Max(0f, GetStat(StatType.ComboDamageMultiplier));

    public void AddSkillPoint(int amount)
    {
        skillPoints = Mathf.Max(0, skillPoints + amount);
    }
    
    public bool UnlockSkill(SkillSO skill)
    {
        if (skill == null)
            return false;

        if (HasSkill(skill))
            return false;

        _skills.Add(new PlayerSkillInstance(skill, 1));
        return true;
    }

    public bool HasSkill(SkillSO skill)
    {
        for(int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].Skill == skill)
                return true; 
        }

        return false;
    }

    public bool UpgradeSkill(SkillSO skill)
    {
        if (skillPoints <= 0)
            return false;

        for(int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].Skill != skill)
                continue;

            if (!_skills[i].Upgrade())
                return false;

            skillPoints--;
            return true;
        }
        return false;
    }

    public List<PlayerSkillInstance> GetUnlockedSkills()
    {
        return new List<PlayerSkillInstance>(_skills);
    }

    public void UnlockSkillsByCurrentLevel()
    {
        if (baseStat == null || baseStat.skillTree == null)
            return;

        List<SkillSO> unlocks = baseStat.skillTree.GetUnlockSkillsUpToLevel(level);

        for (int i = 0; i < unlocks.Count; i++)
            UnlockSkill(unlocks[i]);
    }

    public int GetAttack(Direction dir)
    {
        return Mathf.Max(1, Mathf.RoundToInt(GetRawAttack(dir)));
    }

    public int GetAverageAttack()
    {
        float total =
            GetRawAttack(Direction.Up) +
            GetRawAttack(Direction.Down) +
            GetRawAttack(Direction.Left) +
            GetRawAttack(Direction.Right);

        return Mathf.Max(1, Mathf.RoundToInt(total / 4f));
    }

    private float GetRawAttack(Direction dir)
    {
        StatType attackType = GetAttackStatType(dir);

        float directionAttack = GetStat(attackType);
        float allFlat = GetStat(StatType.AttackAllFlat);
        float allPercent = GetStat(StatType.AttackAllPercent);

        return (directionAttack + allFlat) * (1f + allPercent) * BasicAttackMultiplier;
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

    public int GetLevel()
    {
        return level;
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

            StatType.AttackAllFlat => 0f,
            StatType.AttackAllPercent => 0f,

            StatType.DefensePower => baseStat.defensePower,

            StatType.MaxCost => baseStat.maxCost,
            StatType.StartCost => baseStat.startCost,
            StatType.CostRecovery => baseStat.baseCostRecovery,
            StatType.DefenseSuccessCostBonus => baseStat.defenseSuccessCostBonus,

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

    public PlayerSkillInstance GetSkillInstance(SkillSO skill)
    {
        for(int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].Skill == skill)
                return _skills[i];
        }

        return null;
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

    public bool IsSkillUnlocked(SkillSO skill)
    {
        return GetSkillInstance(skill) != null;
    }
    public void Heal(int amount)
    {
        currentHp = Mathf.Clamp(currentHp + amount, 0, MaxHp);
    }

    private void ClampCurrentHp()
    {
        currentHp = Mathf.Clamp(currentHp, 0, MaxHp);
    }

    public void AddExpRaw(int amount)
    {
        currentExp += Mathf.Max(0, amount);
    }
    public void SpendExp(int amount)
    {
        currentExp = Mathf.Max(0, currentExp - amount);
    }

    public void LevelUp()
    {
        level++;
    }

    //æ∆¿Ã≈€

    public bool RequestEquipItem(PlayerItemInstance itemInstance)
    {
        if (itemInstance == null)
            return false;

        if (!playerInventory.HasItem(itemInstance))
            return false;

        return equipmentSlot.EquipItem(itemInstance);
    }

    public bool RequestUnequipItem(PlayerItemInstance itemInstance)
    {
        if (itemInstance == null)
            return false;

        return equipmentSlot.UnequipItem(itemInstance);
    }

    public void GetItem(ItemSO item)
    {
        playerInventory.ObtainItem(item);
    }

    public void DropItem(PlayerItemInstance itemInstance)
    {
        playerInventory.DeleteItem(itemInstance);
    }

    public void DispatchItemBattleEvent(ItemBattleEventContext context)
    {
        equipmentSlot.DispatchBattleEvent(context);
    }

    //########################### Artifact #############################

    // BattleScene
    public void DispatchArtifactBattleEvent(ArtifactBattleEventContext context)
    {
        playerArtifactCollection.DispatchBattleEvent(context);
    }

    // MapScene
    public void DispatchArifactMapEvent()
    {

    }
}

using ArrowClash.Common;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class BattleEntity : MonoBehaviour
{
    public BaseStatSO statData;
    public int currentHp { get; private set; }
    public StatusHandler statusHandler { get; private set; }

    private PlayerInstance _playerInstance;
    public bool IsPlayerEntity => _playerInstance != null;

    private void Awake()
    {
        statusHandler = new StatusHandler(this);
    }

    public void Initialize(BaseStatSO data)
    {
        _playerInstance = null;
        statData = data;
        currentHp = data.maxHp;
        Debug.Log($"{data.jobName} initialized. HP: {currentHp}");
    }

    public void InitializeFromInstance(PlayerInstance pi)
    {
        _playerInstance = pi;
        statData = pi.baseStat;
        currentHp = pi.currentHp;
        Debug.Log($"Player initialized. HP: {currentHp}");
    }

    public int GetMaxHp()
    {
        return IsPlayerEntity ? _playerInstance.MaxHp : statData.maxHp;
    }

    public int GetAttackPower(Direction dir)
    {
        if (IsPlayerEntity)
            return _playerInstance.GetAttack(dir);
        float value = statData.attackPower.Get(dir) * statData.basicAttackMultiplier;
        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public int GetAverageAttackPower()
    {
        if (IsPlayerEntity)
            return _playerInstance.GetAverageAttack();

        float total =
            statData.attackPower.up +
            statData.attackPower.down +
            statData.attackPower.left +
            statData.attackPower.right;

        return Mathf.Max(1, Mathf.RoundToInt(total / 4f));
    }

    public int GetSkillPower()
    {
        if (IsPlayerEntity)
            return _playerInstance.GetSkillPowerValue();

        float value = GetAverageAttackPower() * statData.skillPowerMultiplier;
        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public int GetStatusPowerValue()
    {
        if (IsPlayerEntity)
            return _playerInstance.GetStatusPowerValue();
        float value = GetAverageAttackPower() * statData.statusPower;
        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public float GetDefensePower()
    {
        return IsPlayerEntity ? _playerInstance.DefensePower : statData.defensePower;
    }
    public float GetComboDamageMultiplier()
    {
        return IsPlayerEntity ? _playerInstance.ComboDamageMultiplier : statData.comboDamageMultiplier;
    }

    public float GetDamageTakenMultiplier()
    {
        return IsPlayerEntity ? _playerInstance.DamageTakenMultiplier : statData.damageTakenMultiplier;
    }
    public void TakeDamage(int finalDamage)
    {
        currentHp -= Mathf.Max(0, finalDamage);
        currentHp = Mathf.Max(currentHp, 0);

        Debug.Log($"{statData.jobName} HP: {currentHp}");

        if (currentHp <= 0)
            OnDie();
    }
    public int GetMaxCost()
    {
        return IsPlayerEntity ? _playerInstance.MaxCost : statData.maxCost;
    }

    public int GetStartCost()
    {
        return IsPlayerEntity ? _playerInstance.StartCost : statData.startCost;
    }

    public int GetCostRecovery()
    {
        return IsPlayerEntity ? _playerInstance.CostRecovery : statData.baseCostRecovery;
    }

    public int GetDefenseSuccessCostBonus()
    {
        return IsPlayerEntity ? _playerInstance.DefenseSuccessCostBonus : statData.defenseSuccessCostBonus;
    }

    public void OnDie()
    {
        Debug.Log($"{statData.jobName} died.");
        TurnManager.instance.OnEntityDied(this);
    }
}

using System.Collections.Generic;
using UnityEngine;
using ArrowClash.Common;
using System.Collections;
using System;
public class BattleActionService
{
    private const int BasicAttackCost = 5;

    private readonly MonoBehaviour _coroutineHost;

    private BattleRuntime _runtime;
    private CostHandler _costHandler;
    private SkillExecutor _skillExecutor;

    public bool IsInitialized { get; private set; }
    public bool IsProcessing { get; private set; }
    public bool IsComboAttack { get; private set; }
    public bool CanUseSkill { get; private set; }
    public int CurrentCost => _costHandler != null ? _costHandler.currentCost : 0;
    public int MaxCost => _costHandler != null ? _costHandler.maxCost : 0;


    public BattleActionService(MonoBehaviour coroutineHost)
    {
        _coroutineHost = coroutineHost;
    }

    public void Initialize(BattleRuntime runtime, List<SkillSO> skills)
    {
        _runtime = runtime;
        _costHandler = new CostHandler(runtime.Player.statData.startCost, runtime.Player.statData.maxCost);
        _skillExecutor = new SkillExecutor(skills ?? new List<SkillSO>());

        IsInitialized = true;
        IsProcessing = false;
        IsComboAttack = false;
        CanUseSkill = false;
    }

    public void EndPlayerTurn()
    {
        CanUseSkill = false;
        IsComboAttack = false;
    }

    public void TryUseSkill()
    {
        if (!IsInitialized || IsProcessing || !CanUseSkill) return;
        if (_runtime.Player.statusHandler.IsSkillDisabled()) return;

        SkillSO skill = _skillExecutor.OnSpaceBar();
        if (skill == null) return;

        if (!_costHandler.SpendCost(skill.cost)) return;

        if (skill.effects == null) return;

        foreach (var effect in skill.effects)
            effect.Apply(_runtime.Player, _runtime.Enemy);
    }

    public void TryPlayerAttack(Direction rawDir, Action onComboAvailable, Action onTurnFinished)
    {
        if (!IsInitialized || IsProcessing) return;

        DirectionInputResult inputResult = _runtime.Player.statusHandler.ModifyDirectionInput(rawDir);

        if (inputResult.cancelSkillBuffer)
            _skillExecutor.CancelInput();

        if (inputResult.isFailed)
        {
            if (inputResult.spendCostOnFail)
                _costHandler.SpendCost(BasicAttackCost);

            if (inputResult.endTurnOnFail)
                onTurnFinished?.Invoke();

            return;
        }

        Direction filteredDir = inputResult.direction;

        if (filteredDir == Direction.None) return;

        _skillExecutor.OnDirectionInput(filteredDir);
        _runtime.Player.statusHandler.OnDirectionInput();

        if (!_costHandler.SpendCost(BasicAttackCost))
        {
            onTurnFinished?.Invoke();
            return;
        }

        _coroutineHost.StartCoroutine(PlayerAttackSequence(filteredDir, onComboAvailable, onTurnFinished));
    }

    private IEnumerator PlayerAttackSequence(Direction playerDir, Action onComboAvailable, Action onTurnFinished)
    {
        IsProcessing = true;

        bool wasCombo = IsComboAttack;
        BattleResult result;

        Debug.Log($"<color=yellow>[Player Attack]</color> Direction: {playerDir}");

        if (wasCombo)
        {
            result = new BattleResult(true);
            IsComboAttack = false;
            Debug.Log("<color=lime>[Combo Attack!]</color>");
        }
        else
        {
            Direction enemyDefDir = _runtime.EnemyController.SelectRandomDirection();
            result = BattleResolver.Resolve(playerDir, enemyDefDir);
        }

        if (result.success)
        {
            DamageContext context = DamageResolver.CreateBasicAttack(_runtime.Player, _runtime.Enemy,
                playerDir,wasCombo);

            int damage = DamageResolver.Apply(context);

            CanUseSkill = true;
            Debug.Log($"Attack succeeded. Damage: {damage}. Remaining cost: {CurrentCost}");
        }

        yield return new WaitForSeconds(0.5f);

        IsProcessing = false;

        if (_runtime.Enemy.currentHp <= 0)
            yield break;

        if (result.success && _costHandler.CanSpend(BasicAttackCost))
        {
            CanUseSkill = true;
            IsComboAttack = true;
            Debug.Log("<color=lime>Combo available!</color>");
            onComboAvailable?.Invoke();
        }
        else
        {
            onTurnFinished?.Invoke();
        }
    }

    public void TryEnemyAttack(Direction enemyDir, Direction playerDir, int turnCount, Action onEnemyTurnResolved)
    {
        if (!IsInitialized || IsProcessing) return;
        _coroutineHost.StartCoroutine(EnemyAttackSequence(enemyDir, playerDir, turnCount, onEnemyTurnResolved));
    }

    private IEnumerator EnemyAttackSequence(Direction enemyDir, Direction playerDir, int turnCount, Action onEnemyTurnResolved)
    {
        IsProcessing = true;

        _runtime.Player.statusHandler.OnDirectionInput();

        DirectionInputResult inputResult = _runtime.Player.statusHandler.ModifyDirectionInput(playerDir);

        if (inputResult.cancelSkillBuffer)
            _skillExecutor.CancelInput();

        if (inputResult.isFailed)
        {
            if (inputResult.spendCostOnFail)
                _costHandler.SpendCost(BasicAttackCost);

            playerDir = Direction.None;
        }
        else
        {
            playerDir = inputResult.direction;
        }

        BattleResult result = BattleResolver.Resolve(enemyDir, playerDir);

        Debug.Log($"<color=orange>[Enemy Attack]</color> Direction: {enemyDir} / Player defense: {playerDir}");

        if (result.success)
        {
            DamageContext context = DamageResolver.CreateBasicAttack(
                _runtime.Enemy,
                _runtime.Player,
                enemyDir,
                false
            );

            int damage = DamageResolver.Apply(context);
            Debug.Log($"<color=red>Defense failed!</color> Took {damage} damage.");
        }
        else
        {
            _costHandler.pendingDefenseBonus = true;
            Debug.Log("<color=green>Defense succeeded! Next turn gets +5 cost.</color>");
        }

        yield return new WaitForSeconds(0.5f);

        IsProcessing = false;

        if (_runtime.Player.currentHp <= 0)
            yield break;

        _costHandler.RecoverOnTurnEnd(_runtime.Player.statData.baseCostRecovery, turnCount);
        _runtime.Enemy.statusHandler.Tick();
        onEnemyTurnResolved?.Invoke();
    }

}

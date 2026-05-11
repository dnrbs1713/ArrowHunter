using ArrowClash.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("Fallback Skills")]
    [SerializeField] private List<SkillSO> skillList;

    private readonly BattleRuntime _runtime = new BattleRuntime();
    private BattleActionService _actions;
    private BattleState _currentState;
    private bool _battleStarted;

    public BattleEntity player => _runtime.Player;
    public BattleEntity enemy => _runtime.Enemy;
    public PlayerCombatController playerController => _runtime.PlayerController;
    public EnemyController enemyController => _runtime.EnemyController;

    public bool isProcessing => _actions != null && _actions.IsProcessing;
    public bool isComboAttack => _actions != null && _actions.IsComboAttack;
    public bool canUseSkill => _actions != null && _actions.CanUseSkill;

    public int turnCount { get; private set; } = 1;
    public int currentCost => _actions != null ? _actions.CurrentCost : 0;
    public int maxCost => _actions != null ? _actions.MaxCost : 0;

    public static event Action<int> OnVictory;
    public static event Action OnDefeat;

    private void Awake()
    {
        instance = this;
        _actions = new BattleActionService(this);
    }

    private void Start()
    {
        TryStartBattle();
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(BattleState newState)
    {
        _currentState?.Exit();
        Input.ResetInputAxes();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void RegisterPlayer(BattleEntity entity, PlayerCombatController controller)
    {
        _runtime.RegisterPlayer(entity, controller);
        TryStartBattle();
    }

    public void RegisterEnemy(BattleEntity entity, EnemyController controller)
    {
        _runtime.RegisterEnemy(entity, controller);
        TryStartBattle();
    }

    private void TryStartBattle()
    {
        if (_battleStarted) return;
        if (!_runtime.IsReady) return;

        List<PlayerSkillInstance> runtimeSkills = new List<PlayerSkillInstance>();

        if (BattleDataManager.instance != null && BattleDataManager.instance.PlayerInstance != null)
        {
            BattleDataManager.instance.PlayerInstance.UnlockSkillsByCurrentLevel();
            runtimeSkills = BattleDataManager.instance.PlayerInstance.GetUnlockedSkills();
        }

        _actions.Initialize(_runtime, runtimeSkills);
        _battleStarted = true;

        Debug.Log("Battle initialized");
        ChangeState(new PlayerAttackState(this, playerController));
    }

    public void TryUseSkill()
    {
        if (!_battleStarted) return;
        _actions.TryUseSkill();
    }

    public void GiveUpCombo()
    {
        if (!_battleStarted) return;
        if (!isComboAttack || isProcessing) return;

        FinishTurn();
    }

    public void HandlePlayerDirectionInput(Direction dir)
    {
        if (!_battleStarted) return;

        _actions.TryPlayerAttack(
            dir,
            () => ChangeState(new PlayerAttackState(this, playerController)),
            FinishTurn
        );
    }

    public void HandleEnemyDefenseInput(Direction enemyDir, Direction playerDir)
    {
        if (!_battleStarted) return;

        _actions.TryEnemyAttack(
            enemyDir,
            playerDir,
            turnCount,
            OnEnemyTurnResolved
        );
    }

    private void OnEnemyTurnResolved()
    {
        turnCount++;
        ChangeState(new PlayerAttackState(this, playerController));
    }

    public void FinishTurn()
    {
        if (!_battleStarted) return;

        _actions.EndPlayerTurn();
        player.statusHandler.Tick();
        ChangeState(new EnemyTurnState(this, enemyController, playerController));
    }

    public void OnEntityDied(BattleEntity entity)
    {
        if (entity == enemy)
            OnVictory?.Invoke(turnCount);
        else if (entity == player)
            OnDefeat?.Invoke();
    }
}
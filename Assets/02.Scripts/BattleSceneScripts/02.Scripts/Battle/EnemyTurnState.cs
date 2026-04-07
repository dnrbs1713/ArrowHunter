using UnityEngine;
using ArrowClash.Common;

public class EnemyTurnState : BattleState
{
    private readonly EnemyController _enemyController;
    private readonly PlayerCombatController _playerController;
    private Direction _enemyAttackDir;

    public EnemyTurnState(TurnManager manager, EnemyController enemy, PlayerCombatController player) : base(manager)
    {
        _enemyController = enemy;
        _playerController = player;
    }

    public override void Enter()
    {
        _enemyAttackDir = _enemyController.SelectRandomDirection();
        Debug.Log("<color=red>[Enemy Turn]</color> Defend now!");
    }

    public override void Update()
    {
        if (manager.isProcessing) return;

        Direction playerDir = _playerController.GetDirectionInput();
        if (playerDir != Direction.None)
            manager.HandleEnemyDefenseInput(_enemyAttackDir, playerDir);
    }
    public override void Exit() { }
}

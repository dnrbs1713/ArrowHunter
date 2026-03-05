using UnityEngine;
using ArrowClash.Common;
public class EnemyTurnState : BattleState
{
    private readonly EnemyController _enemyController;
    private readonly PlayerCombatController _playerController;
    private Direction enemyAttackDir;
    public EnemyTurnState(TurnManager manager, EnemyController enemy, PlayerCombatController player) : base(manager)
    {
        _enemyController = enemy;
        _playerController = player;
    }


    public override void Enter()
    {
        enemyAttackDir = _enemyController.SelectRandomDirection();
        Debug.Log($"<color=red>[적 공격]</color> 적이 공격해옵니다! 방어하세요!");
    }

    public override void Update()
    {
        Direction playerDir = _playerController.GetDirectionInput();
        if(playerDir != Direction.None)
        {
            manager.ExecuteEnemyAttack(enemyAttackDir, playerDir);
        }
    }
}

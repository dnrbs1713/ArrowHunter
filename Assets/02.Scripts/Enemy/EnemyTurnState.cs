using UnityEngine;
using ArrowClash.Common;
public class EnemyTurnState : BattleState
{
    private Direction enemyAttackDir;
    public EnemyTurnState(TurnManager manager) : base(manager) { }

    public override void Enter()
    {
        enemyAttackDir = EnemyController.instance.SelectRandomDirection();
        Debug.Log($"<color=red>[적 공격]</color> 적이 공격해옵니다! 방어하세요!");
    }

    public override void Update()
    {
        Direction playerDir = PlayerCombatController.instance.GetDirectionInput();
        if(playerDir != Direction.None)
        {
            manager.ExecuteEnemyAttack(enemyAttackDir, playerDir);
        }
    }
}

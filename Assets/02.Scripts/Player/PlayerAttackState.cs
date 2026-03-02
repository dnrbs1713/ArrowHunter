using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using ArrowClash.Common;
public class PlayerAttackState : BattleState
{
    public PlayerAttackState(TurnManager manager) : base(manager){ }

    public override void Enter()
    {
        Debug.Log("=== 플레이어 공격 턴 ===");
    }

    public override void Update()
    {

        if (manager.isProcessing) return;

        ArrowClash.Common.Direction dir = PlayerCombatController.instance.GetDirectionInput();
        if (dir != ArrowClash.Common.Direction.None)
        {
            manager.ExecutePlayerAttack(dir);
        }
    }
}

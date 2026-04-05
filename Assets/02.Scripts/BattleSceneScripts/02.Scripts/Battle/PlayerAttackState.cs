using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using ArrowClash.Common;
public class PlayerAttackState : BattleState
{
    private readonly PlayerCombatController _playerController;
    public PlayerAttackState(TurnManager manager, PlayerCombatController player) : base(manager)
    {
        _playerController = player;
    }

    public override void Enter()
    {
        Debug.Log("=== 플레이어 공격 턴 ===");
        Debug.Log($"시직 코스트 {TurnManager.instance.currentCost}");

        if (manager.player.statusHandler.IsActionDisabled())
        {
            Debug.Log("<color=red>[스턴] 행동불능 — 턴 스킵</color>");
            manager.FinishTurn();
        }
    }

    public override void Update()
    {

        if (manager.isProcessing) return;

        ArrowClash.Common.Direction dir = _playerController.GetDirectionInput();
        if (dir != ArrowClash.Common.Direction.None)
        {
            manager.OnDirectionInput(dir);
        }
    }
    public override void Exit() { }
}

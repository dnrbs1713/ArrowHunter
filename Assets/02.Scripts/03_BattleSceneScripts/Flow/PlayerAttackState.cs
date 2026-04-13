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
        Debug.Log("=== Player Turn ===");
        Debug.Log($"Start Cost: {manager.currentCost}");

        if (manager.player.statusHandler.IsActionDisabled())
        {
            Debug.Log("<color=red>[Stun] Action disabled. Skip turn.</color>");
            manager.FinishTurn();
        }
    }

    public override void Update()
    {
        if (manager.isProcessing) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.TryUseSkill();
            return;
        }

        if (manager.isComboAttack && Input.GetKeyDown(KeyCode.Z))
        {
            manager.GiveUpCombo();
            return;
        }

        ArrowClash.Common.Direction dir = _playerController.GetDirectionInput();
        if (dir != ArrowClash.Common.Direction.None)
            manager.HandlePlayerDirectionInput(dir);
    }

    public override void Exit() { }
}
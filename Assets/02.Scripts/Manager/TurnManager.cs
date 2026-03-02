using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using Unity.VisualScripting;
using ArrowClash.Common;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private BattleState currentState;

    public bool isEnemyStunned = false;

    private void Awake() => instance = this;
    private void Start()
    {
        ChangeState(new PlayerAttackState(this));
    }
    private void Update()
    {
        currentState?.Update();
    }
    public void ChangeState(BattleState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void ExecutePlayerAttack(Direction playerDir)
    {
        Direction enemyDefdir = EnemyController.instance.SelectRandomDirection();
        BattleResult result = BattleManager.instance.Resolve(playerDir, enemyDefdir);

        Debug.Log(result.success ? "공격 성공!" : "공격 실패!");

        if (isEnemyStunned)
        {
            isEnemyStunned=false;
            ChangeState(new PlayerAttackState(this));
        }
        else
        {
            ChangeState(new EnemyTurnState(this));
        }
    }
    public void ExecuteEnemyAttack(Direction enemyDir, Direction playerDir)
    {
        BattleResult result = BattleManager.instance.Resolve(enemyDir, playerDir);
        Debug.Log(result.success ? "방어 실패" : "방어 성공!");
        
        ChangeState(new PlayerAttackState(this));
    }
}

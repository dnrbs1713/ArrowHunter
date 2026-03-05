using UnityEngine;
using System.Collections;
using ArrowClash.Common;
using NUnit.Framework;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("Entities")]
    public BattleEntity player;
    public BattleEntity enemy;

    [Header("Status")]
    public bool isProcessing = false; // 코루틴 중복 실행 및 입력 방지용 잠금 장치
    public bool isComboAttack = false;
    public int turnCount = 1;

    [Header("Controllers")]
    public PlayerCombatController playerController;
    public EnemyController enemyController;


    private CostHandler _costHandler;
    private CombatProcessor _combatProcessor = new CombatProcessor();
    private BattleState _currentState;
    public int currentCost => _costHandler.currentCost;
    public int maxCost => _costHandler.maxCost;




    private void Awake() => instance = this;

    private void Start()
    {
        // 게임 시작 시 플레이어 공격 상태로 진입  
        _costHandler = new CostHandler(player.statData.startCost, player.statData.maxCost);
        ChangeState(new PlayerAttackState(this, playerController));
    }

    private void Update()
    {
        _currentState?.Update();

        if(isComboAttack && !isProcessing && Input.GetKey(KeyCode.Z))
        {
            Debug.Log("<color=orange>플레이어가 콤보를 포기합니다. 턴을 넘깁니다.</color>");
            isComboAttack = false;
            ChangeState(new EnemyTurnState(this,enemyController,playerController));
        }
    }

    public void ChangeState(BattleState newState)
    {
        _currentState?.Exit();
        Input.ResetInputAxes(); // 상태 전환 시 입력 버퍼 초기화 (이전 프레임의 입력 잔상 제거)
        _currentState = newState;
        _currentState?.Enter();
    }

    // ********************** 플레이어 공격 로직 **********************
    public void ExecutePlayerAttack(Direction playerDir)
    {
        // 이미 처리 중이라면 중복 실행 방지
        if (isProcessing) return;

        // 공격 비용 체크
        if(!_costHandler.SpendCost(5))
        {
            FinishTurn();
            return;
        }
        StartCoroutine(PlayerAttackSequence(playerDir));
    }

    private IEnumerator PlayerAttackSequence(Direction playerDir)
    {
        isProcessing = true; // 잠금 시작

        bool wasCombo = isComboAttack;
        BattleResult result;

        Debug.Log($"<color=yellow>[플레이어 공격]</color> 방향: {playerDir}");

        if (wasCombo)
        {
            result = new BattleResult(true);
            isComboAttack = false;
            Debug.Log("<color=lime>[콤보 공격!]</color>");
        }
        else
        {
            Direction enemyDefdir = enemyController.SelectRandomDirection();
            result = BattleResolver.Resolve(playerDir, enemyDefdir);
        }

        if (result.success)
        {
            //_combatProcessor.OnAttackSuccess(this);
            int damage = _combatProcessor.CalculateFinalDamage(player, enemy, wasCombo);
            enemy.TakeDamage(damage);
            Debug.Log($"공격 성공! {damage} 데미지. 남은 코스트: {currentCost}");
        }

        yield return new WaitForSeconds(0.5f);

        isProcessing = false; // 잠금 해제

        // 사망 체크 후 상태 전환
        if (enemy.currentHp <= 0)
        {
            // 여기에 승리 상태(VictoryState) 등을 추가할 수 있습니다.
            Debug.Log("전투 승리!");
            yield break;
        }

        if(result.success && _costHandler.CanSpend(5))
        {
            isComboAttack = true;
            Debug.Log("<color=lime> 콤보 가능!</color>");
            ChangeState(new PlayerAttackState(this,playerController));
        }
        else
        {
            FinishTurn();
        }
    }

    // ******************* 적 공격 로직 ***********************
    public void ExecuteEnemyAttack(Direction enemyDir, Direction playerDir)
    {
        if (isProcessing) return;
        StartCoroutine(EnemyAttackSequence(enemyDir, playerDir));
    }

    private IEnumerator EnemyAttackSequence(Direction enemyDir, Direction playerDir)
    {
        isProcessing = true;
        BattleResult result = BattleResolver.Resolve(enemyDir, playerDir);

        Debug.Log($"<color=orange>[적 공격]</color> 방향: {enemyDir} / 플레이어 방어: {playerDir}");

        if (result.success) // 플레이어가 방어하지 못한 경우
        { 
            player.TakeDamage(enemy.statData.attackDamage);
            Debug.Log($"<color=red>방어 실패!</color> {enemy.statData.attackDamage} 데미지를 입었습니다.");
        }
        else
        {
            _costHandler.pendingDefenseBonus = true;
            Debug.Log("<color=green>방어 성공! 다음 턴 코스트 보너스 +5</color>");
        }

        yield return new WaitForSeconds(0.5f);

        isProcessing = false;

        if(player.currentHp <= 0)
        {
            Debug.Log("<color=red> 전투 패배");
            yield break;
        }

        _costHandler.RecoverOnTurnEnd(player.statData.baseCostRecovery, turnCount);
        turnCount++;
        ChangeState(new PlayerAttackState(this,playerController));
    }


    //********************* 턴 종료 / 코스트 계산 *********************//
    public void FinishTurn()
    {
        isComboAttack = false;
        ChangeState(new EnemyTurnState(this,enemyController,playerController));
    }


    public void OnEntityDied(BattleEntity entity)
    {
        if(entity == enemy) Debug.Log("=== 전투 승리! ===");
        
        else if(entity == player) Debug.Log("=== 전투 패배... ===");
    }
}
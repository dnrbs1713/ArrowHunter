using UnityEngine;
using System.Collections;
using ArrowClash.Common;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("Entities")]
    public BattleEntity player;
    public BattleEntity enemy;

    [Header("Status")]
    public bool isEnemyStunned = false;
    public bool isProcessing = false; // 코루틴 중복 실행 및 입력 방지용 잠금 장치

    private BattleState currentState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 게임 시작 시 플레이어 공격 상태로 진입
        ChangeState(new PlayerAttackState(this));
    }

    private void Update()
    {
        // 현재 상태의 Update 로직 실행
        currentState?.Update();
    }

    public void ChangeState(BattleState newState)
    {
        currentState?.Exit();

        // 상태 전환 시 입력 버퍼 초기화 (이전 프레임의 입력 잔상 제거)
        Input.ResetInputAxes();

        currentState = newState;
        currentState?.Enter();
    }

    // --- 플레이어 공격 로직 ---
    public void ExecutePlayerAttack(Direction playerDir)
    {
        // 이미 처리 중이라면 중복 실행 방지
        if (isProcessing) return;
        StartCoroutine(PlayerAttackSequence(playerDir));
    }

    private IEnumerator PlayerAttackSequence(Direction playerDir)
    {
        isProcessing = true; // 잠금 시작

        // 적의 방어 방향 결정 및 결과 판정
        Direction enemyDefdir = EnemyController.instance.SelectRandomDirection();
        BattleResult result = BattleManager.instance.Resolve(playerDir, enemyDefdir);

        Debug.Log($"<color=yellow>[플레이어 공격]</color> 방향: {playerDir} / 적 방어: {enemyDefdir}");

        if (result.success)
        {
            Debug.Log($"<color=cyan>공격 성공!</color> {player.statData.attackDamage} 데미지를 입혔습니다.");
            enemy.TakeDamage(player.statData.attackDamage); //
            Debug.Log($"상대방 남은 HP: {enemy.currentHp}"); //
        }
        else
        {
            Debug.Log("<color=white>공격 실패!</color> 적이 방어했습니다.");
        }

        // 결과 로그를 읽을 수 있도록 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        isProcessing = false; // 잠금 해제

        // 사망 체크 후 상태 전환
        if (enemy.currentHp <= 0)
        {
            // 여기에 승리 상태(VictoryState) 등을 추가할 수 있습니다.
            Debug.Log("전투 승리!");
        }
        else if (isEnemyStunned)
        {
            isEnemyStunned = false;
            ChangeState(new PlayerAttackState(this));
        }
        else
        {
            ChangeState(new EnemyTurnState(this));
        }
    }

    // --- 적 공격 로직 ---
    public void ExecuteEnemyAttack(Direction enemyDir, Direction playerDir)
    {
        if (isProcessing) return;
        StartCoroutine(EnemyAttackSequence(enemyDir, playerDir));
    }

    private IEnumerator EnemyAttackSequence(Direction enemyDir, Direction playerDir)
    {
        isProcessing = true;

        BattleResult result = BattleManager.instance.Resolve(enemyDir, playerDir);

        Debug.Log($"<color=orange>[적 공격]</color> 방향: {enemyDir} / 플레이어 방어: {playerDir}");

        if (result.success) // 플레이어가 방어하지 못한 경우
        {
            Debug.Log($"<color=red>방어 실패!</color> {enemy.statData.attackDamage} 데미지를 입었습니다.");
            player.TakeDamage(enemy.statData.attackDamage);
            Debug.Log($"플레이어 남은 HP: {player.currentHp}");
        }
        else
        {
            Debug.Log("<color=green>방어 성공!</color> 데미지를 입지 않았습니다.");
        }

        yield return new WaitForSeconds(0.5f);

        isProcessing = false;
        ChangeState(new PlayerAttackState(this));
    }
}
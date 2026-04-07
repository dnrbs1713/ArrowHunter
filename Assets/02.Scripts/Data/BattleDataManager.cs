using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager instance;

    [Header("플레이어 SO")]
    public BaseStatSO playerStatSO;

    // 런타임 플레이어 인스턴스 — 버프/아이템 적용
    public PlayerInstance PlayerInstance { get; private set; }

    // 배틀 씬에서 참조할 데이터들
    public BaseStatSO CurrentEnemyStat { get; private set; }
    public bool IsBossBattle { get; private set; }

    private void Awake()
    {
        if(instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // PlayerInstance 생성 — 게임 시작 시 한 번만
        PlayerInstance = new PlayerInstance(playerStatSO);

        MonsterEncounter.OnEncountMonster += SetBattleData;
    }
    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 해제해줍니다.
        MonsterEncounter.OnEncountMonster -= SetBattleData;
    }
    // 이벤트가 발생했을 때 실행될 함수
    private void SetBattleData(BaseStatSO stat, bool isBoss)
    {
        CurrentEnemyStat = stat;
        IsBossBattle = isBoss;
        Debug.Log($"[BattleDataManager] 적 데이터 설정: {stat.jobName}");
        Debug.Log($"SetBattleData stat={stat}, prefab={stat?.prefab}, isBoss={isBoss}");

    }

    // 전투 승리 후 플레이어 HP 동기화
    // TurnManager 승리 이벤트에서 호출
    public void SyncPlayerHp(int remainingHp)
    {
        PlayerInstance.currentHp = remainingHp;
    }
}

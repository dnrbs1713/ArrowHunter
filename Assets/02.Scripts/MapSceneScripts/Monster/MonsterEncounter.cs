using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;

public class MonsterEncounter : MonoBehaviour
{
    [Header("몬스터 데이터")]
    public BaseStatSO monsterStat;
    public bool isBoss = false;

    public event Action OnDefeated;

    private bool _encountered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_encountered) return;
        if (!other.CompareTag("PLAYER")) return;

        _encountered = true;

        // BattleData에 적 정보 저장
        BattleData.playerMapPosition = other.transform.position;
        BattleData.enemyStatData = monsterStat;
        BattleData.isBossBattle = isBoss;

        Debug.Log($"[MonsterEncounter] {monsterStat.jobName} 조우!");
        GameSceneManager.instance.LoadBattle();
    }
    public void NotifyDefeated()
    {
        OnDefeated?.Invoke();
    }
    // 에디터에서 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}

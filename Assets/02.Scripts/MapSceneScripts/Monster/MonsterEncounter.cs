using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;

public class MonsterEncounter : MonoBehaviour
{
    [Header("Monster Data")]
    public BaseStatSO monsterStat;
    public bool isBoss = false;

    public event Action OnDefeated;
    public static event Action<BaseStatSO, bool> OnEncountMonster;

    private bool _encountered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_encountered) return;
        if (!other.CompareTag("PLAYER")) return;

        _encountered = true;
        BattleData.playerMapPosition = other.transform.position;

        Debug.Log($"Encounter stat = {monsterStat}, prefab = {monsterStat?.prefab}");
        OnEncountMonster?.Invoke(monsterStat, isBoss);

        Debug.Log($"[MonsterEncounter] Encountered {monsterStat.jobName}");
        GameSceneManager.instance.LoadBattle();
    }

    public void NotifyDefeated()
    {
        OnDefeated?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}

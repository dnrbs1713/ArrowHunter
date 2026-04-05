using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 프리팹")]
    public GameObject defaultPrefab;

    [Header("스폰 범위 (방 크기 기준)")]
    public float spawnRangeX = 23f;
    public float spawnRangeZ = 23f;
    public float spawnY = 0f;

    [Header("몬스터 간 최소 거리")]
    public float minSpawnDistance = 2f;

    private int _aliveCount = 0;
    private List<Vector3> _usedPositions = new List<Vector3>();

    public void SpawnMonsters(RoomInstance room)
    {
        if(room.data.clearCondition != ClearCondition.KillAllMonsters)
        {
            if (room.data.clearCondition == ClearCondition.None)
                RoomManager.instance.OnRoomCleared();
            return;
        }

        if(room.data.monsters == null || room.data.monsters.Count == 0)
        {
            RoomManager.instance.OnRoomCleared();
            return;
        }

        if (room.isCleared) return;

        _aliveCount = 0;
        _usedPositions.Clear();

        foreach(var spawnData in room.data.monsters)
        {
            if (spawnData.monsterStat == null) continue;

            for(int i = 0; i < spawnData.count; i++)
            {
                SpawnMonster(spawnData.monsterStat, room.data.isBoss);
                _aliveCount++;
            }
        }
        Debug.Log($"[MonsterSpawner] {_aliveCount}마리 스폰 완료");
    }

    private void SpawnMonster(BaseStatSO stat, bool isBoss)
    {
        GameObject prefab = stat.prefab != null ? stat.prefab : defaultPrefab;
        if(prefab == null)
        {
            Debug.LogWarning($"[MonsterSpawner] {stat.jobName} 프리팹 없음");
            _aliveCount = Mathf.Max(0, _aliveCount - 1);  // 스폰 실패 시 카운트 보정
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject monsterObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        //Debug.Log($"몬스터 생성 {spawnPos}");
        MonsterEncounter encounter = monsterObj.GetComponent<MonsterEncounter>();
        if (encounter != null)
        {
            encounter.monsterStat = stat;
            encounter.isBoss = isBoss;
            encounter.OnDefeated += OnMonsterDefeated;
        }
    }

    private void OnMonsterDefeated()
    {
        _aliveCount--;
        Debug.Log($"[MonsterSpawner] 남은 몬스터: {_aliveCount}");

        if(_aliveCount <= 0)
        {
            Debug.Log("[MonsterSpawner] 모든 몬스터 처치!");
            // 포탈 생성은 RoomManager에서 처리
            RoomManager.instance.OnRoomCleared();
        }
    }
    private Vector3 GetRandomSpawnPosition()
    {
        int maxAttempts = 30;

        Vector3 roomCenter = RoomManager.instance.GetCurrentRoomSpawnerPos();


        for(int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(-spawnRangeX , spawnRangeX),
                spawnY,
                Random.Range(-spawnRangeZ, spawnRangeZ));

            candidate += roomCenter;

            // 몬스터 스폰 위치 가까우면 재시도
            if (!IsToolClose(candidate))
            {
                _usedPositions.Add(candidate);
                return candidate;
            }
        }
        Debug.Log($"몬스터 생성 {roomCenter}");
        return roomCenter + new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            spawnY,
            Random.Range(-spawnRangeZ, spawnRangeZ));
            
    }

    private bool IsToolClose(Vector3 candidate)
    {
        foreach(var pos in _usedPositions)
        {
            if (Vector3.Distance(candidate, pos) < minSpawnDistance)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(0, spawnY, 0),
            new Vector3(spawnRangeX * 2, 0.1f, spawnRangeZ * 2));
    }
}

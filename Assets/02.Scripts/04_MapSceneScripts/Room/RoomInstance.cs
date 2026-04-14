using ArrowClash.Common;
using System.Collections.Generic;
using UnityEngine;
public class RoomInstance
{
    // 생성자
    public RoomSO data;
    public Vector2Int gridPos; // position에서 이름 변경 (가독성)
    public Vector3 worldPos;   // 월드 좌표를 데이터에 포함!
    public bool isCleared = false;
    public bool rewardClaimed = false;

    private bool _monstersInitialized = false;
    private readonly List<BaseStatSO> _remainingMonsters = new List<BaseStatSO>();

    public IReadOnlyList<BaseStatSO> RemainingMonsters => _remainingMonsters;


    public RoomInstance(RoomSO data, Vector2Int girdPos, Vector3 worldpos)
    {
        this.data = data;
        this.gridPos = girdPos;
        this.worldPos = worldpos;
    }
    public void InitializeMonsters()
    {
        if (_monstersInitialized) return;

        _remainingMonsters.Clear();

        if (data.monsters != null)
        {
            foreach (var spawnData in data.monsters)
            {
                if (spawnData.monsterStat == null) continue;

                for (int i = 0; i < spawnData.count; i++)
                    _remainingMonsters.Add(spawnData.monsterStat);
            }
        }

        _monstersInitialized = true;
    }
    public void MarkMonsterDefeated(BaseStatSO defeatedStat)
    {
        InitializeMonsters();

        int index = _remainingMonsters.IndexOf(defeatedStat);
        if (index >= 0)
            _remainingMonsters.RemoveAt(index);
    }
    public bool IsClearConditionMet()
    {
        if (data.clearCondition == ClearCondition.None)
            return true;

        if (data.clearCondition == ClearCondition.KillAllMonsters)
            return _remainingMonsters.Count <= 0;

        return false;
    }

    public bool IsOpen(Direction dir) => data.IsOpen(dir);
    public RoomType RoomType => data.roomType;
}

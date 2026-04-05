using UnityEngine;
using System.Collections.Generic;
using ArrowClash.Common;
using Unity.VisualScripting;
public class RoomGenerator
{
    private List<RoomSO> _roomPools;
    private int _maxRetry = 30;
    private float _bossChance = 0.05f;
    private float _eliteChance = 0.10f;

    public RoomGenerator(List<RoomSO> tilePools)
    {
        _roomPools = tilePools;
    }

    public void UpdateDifficulty(int clearCount)
    {
        _bossChance = Mathf.Min(0.05f+ clearCount * 0.03f, 0.30f);
        _eliteChance = Mathf.Min(0.10f + clearCount * 0.02f, 0.40f);
    }

    public RoomSO GenerateRoom(Direction fromDir)
    {
        RoomType targetType = RollTileType();
        List<RoomSO> candidates = _roomPools.FindAll(t => t.roomType == targetType);

        if (candidates.Count == 0) candidates = _roomPools;

        for(int i = 0; i < _maxRetry; i++)
        {
            RoomSO candidate = candidates[Random.Range(0, candidates.Count)];

            if (fromDir == Direction.None || candidate.IsOpen(RoomSO.Opposite(fromDir)))
                return candidate;
        }
        return _roomPools.Find(r => r.roomType == RoomType.Common) ?? _roomPools[0];
    }
    private RoomType RollTileType()
    {
        /*
        float roll = Random.value;

        if (roll < _bossChance) return RoomType.Boss;
        if (roll < _bossChance + _eliteChance) return RoomType.Elite;

        float remaining = 1f - _bossChance - _eliteChance;
        float roll2 = Random.Range(0f, remaining);

        if (roll2 < remaining * 0.5f) return RoomType.Common;
        if (roll2 < remaining * 0.8f) return RoomType.Event;
        */
        //return RoomType.Shop;
        return RoomType.Common;
    }
}

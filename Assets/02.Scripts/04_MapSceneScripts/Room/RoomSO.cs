using UnityEngine;
using ArrowClash.Common;
using System.Collections.Generic;

[System.Serializable]
public class MonsterSpawnData
{
    public BaseStatSO monsterStat;
    public int count = 1;
}

[CreateAssetMenu(fileName = "RoomSO", menuName = "Scriptable Objects/RoomSO")]
public class RoomSO : ScriptableObject
{
    [Header("방 정보")]
    public RoomType roomType;
    public ClearCondition clearCondition;
    public float roomSize;

    [Header("열린 방향")]
    public bool openUp;
    public bool openDown;
    public bool openLeft;
    public bool openRight;

    [Header("몬스터 (Battle/Elite/Boss만 사용)")]
    public List<MonsterSpawnData> monsters;
    public bool isBoss = false;

    [Header("방 프리팹(타입별 교체 사용)")]
    public GameObject roomPrefab;

    public bool IsOpen(Direction dir)
    {
        return dir switch
        {
            Direction.Up => openUp,
            Direction.Down => openDown,
            Direction.Left => openLeft,
            Direction.Right => openRight,
            _ => false
        };
    }

    public static Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.None
        };
    }
}

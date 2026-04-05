using UnityEngine;
using ArrowClash.Common;
public static class BattleData
{
    // πË∆≤ æ¿
    public static bool isVictory;
    public static int turnCount;
    public static RoomType currentRoomType;
    public static BaseStatSO enemyStatData;   // √ﬂ∞°
    public static bool isBossBattle;    // √ﬂ∞°

    // ∏  æ¿
    public static Vector3 playerMapPosition;
    public static Direction enterDirection;
    public static Vector3 currentRoomWorldPos;
}

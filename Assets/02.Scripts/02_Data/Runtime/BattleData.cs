using UnityEngine;
using ArrowClash.Common;

public static class BattleData
{
    public static bool isVictory;
    public static int turnCount;

    public static Vector3 playerMapPosition;
    public static Direction enterDirection;
    public static Vector3 currentRoomWorldPos;

    public static bool hasPendingBattleVictory;
    public static BaseStatSO defeatedEnemyStat;

    public static int gainedExp;
    public static int levelBefore;
    public static int levelAfter;
    public static int levelUpCount;

    public static void SetBattleVictory(
        int turn,
        BaseStatSO defeatedEnemy,
        ProgressionResult progression)
    {
        isVictory = true;
        turnCount = turn;

        hasPendingBattleVictory = true;
        defeatedEnemyStat = defeatedEnemy;

        gainedExp = progression.gainedExp;
        levelBefore = progression.levelBefore;
        levelAfter = progression.levelAfter;
        levelUpCount = progression.levelUpCount;
    }

    public static void ClearPendingBattleVictory()
    {
        hasPendingBattleVictory = false;
        defeatedEnemyStat = null;

        gainedExp = 0;
        levelBefore = 0;
        levelAfter = 0;
        levelUpCount = 0;
    }
}

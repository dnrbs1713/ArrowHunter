using UnityEngine;

public struct ProgressionResult
{
    public int gainedExp;
    public int levelBefore;
    public int levelAfter;
    public int levelUpCount;
}

public class PlayerProgressionService : MonoBehaviour
{
    [SerializeField] private PlayerGrowthSO growthSO;
    public ProgressionResult GrantExpFromCurrentEnemy()
    {
        BaseStatSO enemyStat = BattleDataManager.instance.CurrentEnemyStat;
        int exp = enemyStat != null ? enemyStat.expReward : 0;

        return GrantExpAndApplyLevelUps(exp);
    }
    public ProgressionResult GrantExpAndApplyLevelUps(int exp)
    {
        PlayerInstance player = BattleDataManager.instance.PlayerInstance;

        ProgressionResult result = new ProgressionResult
        {
            gainedExp = exp,
            levelBefore = player.level,
            levelAfter = player.level,
            levelUpCount = 0
        };

        player.AddExpRaw(exp);

        while (player.currentExp >= growthSO.GetRequiredExp(player.level))
        {
            int requiredExp = growthSO.GetRequiredExp(player.level);

            player.SpendExp(requiredExp);
            player.LevelUp();

            foreach (var modifier in growthSO.CreateLevelUpModifiers(player.level))
                player.AddModifier(modifier);

            result.levelUpCount++;
        }

        result.levelAfter = player.level;
        return result;
    }
}

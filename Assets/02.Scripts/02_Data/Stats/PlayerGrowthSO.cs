using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerGrowthSO", menuName = "Scriptable Objects/PlayerGrowthSO")]
public class PlayerGrowthSO : ScriptableObject
{
    [Header("경험치 공식")]
    public int baseRequiredExp = 10;
    public int requiredExpInceresePerLevel = 5;

    [Header("레벨업마다 자동 적용할 스탯")]
    public List<StatModifier> levelUpModifiers;

    public int GetRequiredExp(int level)
    {
        return baseRequiredExp + (level - 1) * requiredExpInceresePerLevel;
    }

    public IEnumerable<StatModifier> CreateLevelUpModifiers(int newLevel)
    {
        foreach(var modifier in levelUpModifiers)
        {
            yield return new StatModifier(
                modifier.statType,
                modifier.mode,
                modifier.value,
                -1,
                $"LevelUp_{newLevel}"
            );
        }
    }
}

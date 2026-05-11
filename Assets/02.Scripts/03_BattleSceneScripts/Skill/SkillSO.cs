using UnityEngine;
using ArrowClash.Common;
using System.Collections.Generic;

[System.Serializable]
public class SkillLevelData
{
    public int level = 1;
    public int cost = 0;

    [TextArea]
    public string description;

    public List<SkillEffectSO> effects;
}

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO")]
public class SkillSO : ScriptableObject
{
    public string skillId;
    public string skillName;
    public Sprite icon;

    [TextArea]
    public string description;

    public List<Direction> inputCombo;
    public int maxLevel = 1;
    public List<SkillLevelData> levelData;

    public SkillLevelData GetLevelData(int level)
    {
        if (levelData == null || levelData.Count == 0)
            return null;

        int clampedLevel = Mathf.Clamp(level, 1, maxLevel);

        for(int i = 0; i < levelData.Count; i++)
        {
            if (levelData[i].level == clampedLevel)
                return levelData[i];
        }
        int index = Mathf.Clamp(clampedLevel - 1, 0, levelData.Count - 1);
        return levelData[index];
    }

    public int GetCost(int level)
    {
        SkillLevelData data = GetLevelData(level);
        return data != null ? data.cost : 0;
    }

    public List<SkillEffectSO> GetEffects(int level)
    {
        SkillLevelData data = GetLevelData(level);
        if(data == null || data.effects == null)
            return new List<SkillEffectSO>();

        return data.effects;
    }

}

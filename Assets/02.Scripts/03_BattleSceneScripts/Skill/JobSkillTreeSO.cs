using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class SkillUnlockData
{
    public int unlockLevel = 1;
    public SkillSO skill;
}
[CreateAssetMenu(fileName = "JobSkillTreeSO", menuName = "Scriptable Objects/JobSkillTreeSO")]
public class JobSkillTreeSO : ScriptableObject
{
    public List<SkillUnlockData> unlocks;

    public List<SkillSO> GetUnlockSkillsUpToLevel(int level)
    {
        List<SkillSO> result = new List<SkillSO>();

        if (unlocks == null)
            return result;

        for(int i = 0; i < unlocks.Count; i++)
        {
            SkillUnlockData data = unlocks[i];

            if (data.skill == null)
                continue;

            if (data.unlockLevel <= level)
                result.Add(data.skill);
        }

        return result;
    }

    public List<SkillUnlockData> GetAllUnlocks()
    {
        if (unlocks == null)
            return new List<SkillUnlockData>();

        return new List<SkillUnlockData>(unlocks);
    }
}

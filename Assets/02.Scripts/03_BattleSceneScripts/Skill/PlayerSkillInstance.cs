using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

[SerializeField]
public class PlayerSkillInstance
{
    [SerializeField] private SkillSO skill;
    [SerializeField] private int level;

    public SkillSO Skill => skill;
    public int Level => level;

    public int Cost => skill != null ? skill.GetCost(level) : 0;
    public List<SkillEffectSO> Effects => skill != null ? skill.GetEffects(level) : new List<SkillEffectSO>();
    public bool IsMaxLevel => skill != null && level >= skill.maxLevel;

    public PlayerSkillInstance(SkillSO skill , int startLevel = 1)
    {
        this.skill = skill;
        level = Mathf.Max(1, startLevel);
    }

    public bool CanUpgrade()
    {
        return skill != null && level < skill.maxLevel;
    }

    public bool Upgrade()
    {
        if (!CanUpgrade())
            return false;
        level++;
        return true;
    }
}

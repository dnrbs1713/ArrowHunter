using UnityEngine;
using ArrowClash.Common;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public int cost;
    public List<Direction> inputCombo;
    public List<SkillEffectSO> effects;
}

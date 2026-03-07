using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/CharacterDataSO")]
public class BaseStatSO : ScriptableObject
{
    [Header("기본 정보")]
    public string jobName;

    [Header("전투 능력치")]
    public int maxHp;
    public int attackDamage;
    public int defensePower;

    [Header("코스트")]
    public int maxCost = 99;
    public int startCost;
    public int baseCostRecovery;

    //public List<SkillSO> skillList;
}

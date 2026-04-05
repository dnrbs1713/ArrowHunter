using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/CharacterDataSO")]
public class BaseStatSO : ScriptableObject
{
    [Header("기본 정보")]
    public string jobName;

    [Header("프리팹")]
    public GameObject prefab;

    [Header("전투 능력치")]
    public int maxHp;
    public int attackDamage;
    [Range(0f, 1f)]
    public float defensePower;

    [Header("코스트")]
    public int maxCost = 99;
    public int startCost;
    public int baseCostRecovery;

    public List<SkillSO> skillList;
}

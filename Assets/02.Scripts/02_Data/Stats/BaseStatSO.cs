using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/CharacterDataSO")]
public class BaseStatSO : ScriptableObject
{
    [Header("기본 정보")]
    public string jobName;
    public GameObject prefab;

    [Header("전투 능력치")]
    public int maxHp = 100;

    [Header("방향별 공격력")]
    public DirectionalInt attackPower;

    [Range(0f, 1f)]
    public float defensePower;

    [Header("코스트")]
    public int maxCost = 99;
    public int startCost;
    public int baseCostRecovery;

    [Header("상태이상")]
    public float statusPower = 1f;

    [Range(0f, 1f)]
    public float statusResistance = 0f;

    [Header("배율")]
    public float basicAttackMultiplier = 1f;
    public float skillPowerMultiplier = 1f;
    public float damageTakenMultiplier = 1f;
    public float comboDamageMultiplier = 1.1f;

    [Header("보상")]
    public int expReward = 5;

    [Header("스킬")]
    public List<SkillSO> skillList;
}

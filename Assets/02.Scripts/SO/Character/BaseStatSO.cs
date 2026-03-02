using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/CharacterDataSO")]
public class BaseStatSO : ScriptableObject
{
    [Header("기본 정보")]
    public string jobName;

    [Header("전투 능력치")]
    public int maxHp;
    public int attackDamage;
}

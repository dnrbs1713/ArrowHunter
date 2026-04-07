using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class PlayerInstance : MonoBehaviour
{
    //기본 스탯
    public int baseMaxHp;
    public int baseAttack;
    public float baseDefense;

    //런타임 상태
    public int currentHp;

    //버프/아이템 보너스
    public int bonusMaxHp = 0;
    public int bonusAttack = 0;
    public float bonusDefense = 0;

    //최종 스탯 = 기본값 + 보넛
    public int FinalMaxHp => baseMaxHp + bonusMaxHp;
    public int FinalAttack => baseAttack + bonusAttack;
    public float FinalDefense => baseDefense + bonusDefense;

    public PlayerInstance(BaseStatSO so)
    {
        baseMaxHp = so.maxHp;
        baseAttack = so.attackDamage;
        baseDefense = so.defensePower;
        currentHp = so.maxHp;
    }

    public void TakeDamage(int amount)
    {
        currentHp = Mathf.Max(0, currentHp - amount);
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Max(FinalMaxHp, currentHp + amount);
    }


    // 버프 적용
    public void AddBonusAttack(int amount) => bonusAttack += amount;
    public void AddBonusMaxHp(int amount)
    {
        bonusMaxHp += amount;
        currentHp += amount;  // 최대 HP 증가 시 현재 HP도 증가
    }
    public void AddBonusDefense(float amount) => bonusDefense += amount;

    // 버프 제거
    public void RemoveBonusAttack(int amount) => bonusAttack = Mathf.Max(0, bonusAttack - amount);
    public void RemoveBonusDefense(float amount) => bonusDefense = Mathf.Max(0f, bonusDefense - amount);
}

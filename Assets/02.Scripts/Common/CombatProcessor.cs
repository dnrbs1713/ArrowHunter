using UnityEngine;

public class CombatProcessor
{
    public int CalculateFinalDamage(BattleEntity attacker, BattleEntity defender, bool isCombo)
    {
        int damage = attacker.statData.attackDamage;

        if (isCombo) damage = Mathf.RoundToInt(damage * 1.1f);

        Debug.Log($"{attacker}가 {damage}만큼 피해를 입혔습니다.");
        //방어력 추가
        //int defense = defender.statData

        return damage;
    }
}

using UnityEngine;

public class CombatProcessor
{
    public int CalculateFinalDamage(BattleEntity attacker, BattleEntity defender, bool isCombo)
    {
        float damage = attacker.statData.attackDamage;

        if (isCombo) damage *= 1.1f;

        damage *= (1f - defender.statData.defensePower);

        int final = Mathf.Max(1, Mathf.RoundToInt(damage));

        Debug.Log($"{attacker}가 {damage}만큼 피해를 입혔습니다.");
        Debug.Log($"[데미지] {attacker.statData.jobName} → {defender.statData.jobName} : {final} (콤보: {isCombo})");
        return final;
    }
}

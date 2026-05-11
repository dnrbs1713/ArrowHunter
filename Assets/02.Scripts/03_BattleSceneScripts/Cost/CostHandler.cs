using UnityEngine;

public class CostHandler
{
    public int currentCost { get; private set; }
    public int maxCost { get; private set; }
    public bool pendingDefenseBonus { get; set; } = false;

    private int DefenseSuccessBonus = 2;
    public CostHandler(int startCost, int max)
    {
        currentCost = startCost;
        maxCost = max;
    }

    public bool CanSpend(int amount) => currentCost >= amount;

    public bool SpendCost(int amount)
    {
        if (!CanSpend(amount))
        {
            Debug.Log($"<color=orange>코스트 부족! 필요: {amount} / 현재: {currentCost}");
            return false;
        }
        currentCost -= amount;
        Debug.Log($"코스트 -{amount} 소모. 남은 코스트: {currentCost}");
        return true;
    }
    public void RecoverCost(int amount)
    {
        int before = currentCost;
        currentCost = Mathf.Min(currentCost + amount, maxCost);
        Debug.Log($"코스트 +{currentCost - before} 회복. 현재: {currentCost}");
    }

    public void RecoverOnTurnEnd(int baseCostRecovery, int defenseSuccessBonus)
    {
        int defenseBonus = pendingDefenseBonus ? defenseSuccessBonus : 0;
        int recovery = baseCostRecovery + defenseBonus;

        pendingDefenseBonus = false;
        RecoverCost(recovery);
    }

}

// 코스트 보너스
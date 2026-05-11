using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RarityWeight
{
    public RewardRarity rarity;
    public int weight;
}

[CreateAssetMenu(fileName = "RewardPoolSO", menuName = "Scriptable Objects/RewardPoolSO")]
public class RewardPoolSO : ScriptableObject
{
    public List<RewardSO> rewards;

    [Header("등급별 등장 가중치")]
    public List<RarityWeight> rarityWeights = new List<RarityWeight>
    {
        new RarityWeight { rarity = RewardRarity.Common, weight = 70 },
        new RarityWeight { rarity = RewardRarity.Uncommon, weight = 20 },
        new RarityWeight { rarity = RewardRarity.Rare, weight = 8 },
        new RarityWeight { rarity = RewardRarity.Epic, weight = 2 },
        new RarityWeight { rarity = RewardRarity.Legendary, weight = 0 },
        new RarityWeight { rarity = RewardRarity.Mythic, weight = 0 }
    };

    public List<RewardSO> GetRandomRewards(int count)
    {
        //List<RewardSO> pool = new List<RewardSO>(rewards);
        List<RewardSO> result = new List<RewardSO>();

        List<RewardSO> available = new List<RewardSO>();

        foreach (var reward in rewards)
        {
            if (reward != null)
                available.Add(reward);
        }

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            RewardRarity selectedRarity = PickRarity();
            RewardSO selectedReward = PickRewardByRarity(available, selectedRarity);


            if (selectedReward == null)
                selectedReward = PickAnyReward(available);

            result.Add(selectedReward);
            available.Remove(selectedReward);
        }

        return result;
    }

    private RewardRarity PickRarity()
    {
        int totalWeight = 0;

        foreach (var rarityWeight in rarityWeights)
            totalWeight += Mathf.Max(0, rarityWeight.weight);

        if (totalWeight <= 0)
            return RewardRarity.Common;

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var rarityWeight in rarityWeights)
        {
            current += Mathf.Max(0, rarityWeight.weight);

            if (roll < current)
                return rarityWeight.rarity;
        }

        return RewardRarity.Common;
    }

    private RewardSO PickRewardByRarity(List<RewardSO> available, RewardRarity rarity)
    {
        List<RewardSO> candidates = new List<RewardSO>();

        foreach (var reward in available)
        {
            if (reward.rarity == rarity)
                candidates.Add(reward);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private RewardSO PickAnyReward(List<RewardSO> available)
    {
        if (available.Count == 0)
            return null;

        return available[Random.Range(0, available.Count)];
    }
}

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RewardPoolSO", menuName = "Scriptable Objects/RewardPoolSO")]
public class RewardPoolSO : ScriptableObject
{
    public List<RewardSO> rewards;

    public List<RewardSO> GetRandomRewards(int count)
    {
        List<RewardSO> pool = new List<RewardSO>(rewards);
        List<RewardSO> result = new List<RewardSO>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }
}

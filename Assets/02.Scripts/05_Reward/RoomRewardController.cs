using System.Collections.Generic;
using UnityEngine;

public class RoomRewardController : MonoBehaviour
{
    [SerializeField] private RewardPoolSO rewardPool;
    [SerializeField] private RewardSelectionUI rewardUI;
    [SerializeField] private int rewardChoiceCount = 3;

    private void OnEnable()
    {
        RoomManager.OnRoomClearedEvent += ShowRewardsIfNeeded;
    }

    private void OnDisable()
    {
        RoomManager.OnRoomClearedEvent -= ShowRewardsIfNeeded;
    }

    private void ShowRewardsIfNeeded(RoomInstance room)
    {
        if (room == null) return;
        if (room.rewardClaimed) return;
        if (room.data.clearCondition == ClearCondition.None) return;

        List<RewardSO> rewards = rewardPool.GetRandomRewards(rewardChoiceCount);
        rewardUI.Show(rewards, reward => OnRewardSelected(room, reward));

        Time.timeScale = 0f;
    }

    private void OnRewardSelected(RoomInstance room, RewardSO reward)
    {
        reward.Apply(BattleDataManager.instance.PlayerInstance);

        room.rewardClaimed = true;
        Time.timeScale = 1f;
    }
}

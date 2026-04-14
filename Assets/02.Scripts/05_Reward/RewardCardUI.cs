using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private RewardSO _reward;
    private Action<RewardSO> _onClick;

    public void Set(RewardSO reward, Action<RewardSO> onClick)
    {
        _reward = reward;
        _onClick = onClick;

        if (reward.icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = reward.icon;
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
        nameText.text = reward.rewardName;
        descriptionText.text = reward.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(_reward));
    }
}

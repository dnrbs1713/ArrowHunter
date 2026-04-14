using System;
using System.Collections.Generic;
using UnityEngine;
public class RewardSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<RewardCardUI> cards;

    private Action<RewardSO> _onSelected;
    private void Awake()
    {
        panel.SetActive(false);
    }
    public void Show(List<RewardSO> rewards, Action<RewardSO> onSelected)
    {
        _onSelected = onSelected;
        panel.SetActive(true);

        for (int i = 0; i < cards.Count; i++)
        {
            if (i < rewards.Count)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Set(rewards[i], Select);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    private void Select(RewardSO reward)
    {
        panel.SetActive(false);
        _onSelected?.Invoke(reward);
    }
}

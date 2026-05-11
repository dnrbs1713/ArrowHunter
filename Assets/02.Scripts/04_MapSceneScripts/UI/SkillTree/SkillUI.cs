using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;
public class SkillUI : MonoBehaviour
{
    [SerializeField] private Image  icon;
    [SerializeField] private Button button;
    //[SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject lockedOverlay;

    private SkillUnlockData _unlockData;
    private Action<SkillUnlockData> _onClick;

    public void Set(
        SkillUnlockData unlockData,
        PlayerSkillInstance skillInstance,
        int playerLevel,
        Action<SkillUnlockData> onClick)
    {
        _unlockData = unlockData;
        _onClick = onClick;

        SkillSO skill = unlockData.skill;

        if (icon != null)
        {
            if (skill.icon != null)
            {
                icon.gameObject.SetActive(true);
                icon.sprite = skill.icon;
            }
            else
            {
                icon.gameObject.SetActive(false);
            }
        }

        bool canUnlockByLevel = playerLevel >= unlockData.unlockLevel;
        bool isUnlocked = skillInstance != null;

        //nameText.text = skill.skillName;

        if (isUnlocked)
            levelText.text = $"Lv.{skillInstance.Level} / {skill.maxLevel}";
        else
            levelText.text = $"Lv.{unlockData.unlockLevel} ÇØ±Ý";

        if (lockedOverlay != null)
            lockedOverlay.SetActive(!canUnlockByLevel);

        button.interactable = canUnlockByLevel;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(_unlockData));

    }
}

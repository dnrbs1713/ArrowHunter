using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using TMPro;


public class SkillLevelLineUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform skillRoot;

    public Transform SkillRoot => skillRoot;

    public void SetLevel(int level)
    {
        levelText.text = $"Lv.{level}";
    }
}

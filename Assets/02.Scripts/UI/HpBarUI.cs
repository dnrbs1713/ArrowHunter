using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.UI;
public class HpBarUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private BattleEntity target;
    void Update()
    {
        hpBar.fillAmount = (float) target.currentHp / target.statData.maxHp;
    }
}

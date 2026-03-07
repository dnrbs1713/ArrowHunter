using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using TMPro;

public class CostUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costText;
    void Update()
    {
        costText.text = $"{TurnManager.instance.currentCost} / {TurnManager.instance.maxCost}";
    }
}

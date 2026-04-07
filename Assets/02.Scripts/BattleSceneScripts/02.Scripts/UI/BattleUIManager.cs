using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;

    [Header("HP Bar")]
    [SerializeField] private Image playerHpBar;
    [SerializeField] private Image enemyHpBar;

    [Header("Cost")]
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Victory")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryTurnText;
    public TextMeshProUGUI victoryPromptText;

    [Header("Defeat")]
    public GameObject defeatPanel;
    public TextMeshProUGUI defeatPromptText;

    private void Awake()
    {
        instance = this;
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    private void OnEnable()
    {
        TurnManager.OnVictory += ShowVictory;
        TurnManager.OnDefeat += ShowDefeat;
    }
    private void OnDisable()
    {
        TurnManager.OnVictory -= ShowVictory;
        TurnManager.OnDefeat -= ShowDefeat;
    }

    private void Update()
    {
        RefreshHpBars();
        RefreshCost();
    }

    private void RefreshHpBars()
    {
        if (TurnManager.instance == null) return;

        var player = TurnManager.instance.player;
        var enemy = TurnManager.instance.enemy;

        if (player == null || enemy == null) return;
        if (player.statData == null || enemy.statData == null) return;

        playerHpBar.fillAmount = (float)player.currentHp / player.statData.maxHp;
        enemyHpBar.fillAmount = (float)enemy.currentHp / enemy.statData.maxHp;
    }

    private void RefreshCost()
    {
        if (costText == null) return;
        if (TurnManager.instance == null) return;
        if (TurnManager.instance.player == null || TurnManager.instance.enemy == null) return;
        costText.text = $"{ TurnManager.instance.currentCost} / { TurnManager.instance.maxCost}";
    }

    public void ShowVictory(int turnCount)
    {
        victoryPanel.SetActive(true);
        victoryTurnText.text = $"클리어 턴: {turnCount}";
        victoryPromptText.text = "아무 버튼을 눌러 계속";

        BattleData.isVictory = true;
        BattleData.turnCount = turnCount;

        Time.timeScale = 0f;
    }

    public void ShowDefeat()
    {
        defeatPanel.SetActive(true);
        defeatPromptText.text = "아무 버튼을 눌러 계속";

        BattleData.isVictory = false;
        BattleData.turnCount = TurnManager.instance.turnCount;

        Time.timeScale = 0f;
    }
}

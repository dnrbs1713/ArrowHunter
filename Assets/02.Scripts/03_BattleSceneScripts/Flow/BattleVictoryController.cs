using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class BattleVictoryController : MonoBehaviour
{
    [SerializeField] private PlayerProgressionService progressionService;

    private void OnEnable()
    {
        TurnManager.OnVictory += HandleVictory;
    }

    private void OnDisable()
    {
        TurnManager.OnVictory -= HandleVictory;
    }

    private void HandleVictory(int turnCount)
    {
        BaseStatSO defeatedEnemy = BattleDataManager.instance.CurrentEnemyStat;

        if (TurnManager.instance != null && TurnManager.instance.player != null)
            BattleDataManager.instance.SyncPlayerHp(TurnManager.instance.player.currentHp);

        ProgressionResult progression = progressionService.GrantExpFromCurrentEnemy();

        BattleData.SetBattleVictory(turnCount, defeatedEnemy, progression);

        BattleUIManager.instance.ShowVictory(turnCount, progression);

        GameSceneManager.instance.LoadMap();
    }
}

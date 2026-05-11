using UnityEngine;

public class BattleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public bool isPlayer;

    private void Start()
    {
        SpawnUnit();
    }

    private void SpawnUnit()
    {
        if (isPlayer)
            SpawnPlayer();
        else
            SpawnMonster();
    }

    private void SpawnPlayer()
    {
        BaseStatSO playerSO = BattleDataManager.instance.playerStatSO;
        if (playerSO?.prefab == null) return;

        GameObject obj = Instantiate(playerSO.battlePrefab, transform.position, transform.rotation);
        obj.name = "Player";

        BattleEntity entity = obj.GetComponentInChildren<BattleEntity>();
        PlayerCombatController controller = obj.GetComponentInChildren<PlayerCombatController>();

        if (entity == null || controller == null)
        {
            Debug.LogError("Player prefab is missing BattleEntity or PlayerCombatController.");
            return;
        }

        entity.InitializeFromInstance(BattleDataManager.instance.PlayerInstance);
        controller.playerEntity = entity;
        TurnManager.instance?.RegisterPlayer(entity, controller);
    }

    private void SpawnMonster()
    {
        BaseStatSO enemySO = BattleDataManager.instance.CurrentEnemyStat;
        if (enemySO?.prefab == null) return;

        GameObject obj = Instantiate(enemySO.prefab, transform.position, transform.rotation);
        obj.name = enemySO.jobName;

        BattleEntity entity = obj.GetComponentInChildren<BattleEntity>();
        EnemyController controller = obj.GetComponentInChildren<EnemyController>();

        if (entity == null || controller == null)
        {
            Debug.LogError("Monster prefab is missing BattleEntity or EnemyController.");
            return;
        }

        entity.Initialize(enemySO);
        controller.enemyEntity = entity;
        TurnManager.instance?.RegisterEnemy(entity, controller);
    }
}

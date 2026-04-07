using UnityEngine;

public class BattleRuntime
{
    public BattleEntity Player { get; private set; }
    public BattleEntity Enemy { get; private set; }
    public PlayerCombatController PlayerController { get; private set; }
    public EnemyController EnemyController { get; private set; }

    public bool IsReady =>
        Player != null &&
        Enemy != null &&
        PlayerController != null &&
        EnemyController != null &&
        Player.statData != null &&
        Enemy.statData != null;
    public void RegisterPlayer(BattleEntity entity, PlayerCombatController controller)
    {
        Player = entity;
        PlayerController = controller;
    }

    public void RegisterEnemy(BattleEntity entity, EnemyController controller)
    {
        Enemy = entity;
        EnemyController = controller;
    }

}

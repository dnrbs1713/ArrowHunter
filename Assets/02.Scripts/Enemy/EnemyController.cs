using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public BattleEntity enemyEntity;

    private void Awake()
    {
        instance = this;
        if (enemyEntity == null)
        {
            enemyEntity = GetComponent<BattleEntity>();
        }

    }
    private void Start()
    {
        enemyEntity.Initialize(enemyEntity.statData);
    }
    public Direction SelectRandomDirection()
    {
        return (Direction)Random.Range(1, 5);
    }
}

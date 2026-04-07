using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class EnemyController : MonoBehaviour
{

    public BattleEntity enemyEntity;

    private void Awake()
    {
        if (enemyEntity == null)
        {
            enemyEntity = GetComponent<BattleEntity>();
        }

    }
    public Direction SelectRandomDirection()
    {
        return (Direction)Random.Range(1, 5);
    }
}

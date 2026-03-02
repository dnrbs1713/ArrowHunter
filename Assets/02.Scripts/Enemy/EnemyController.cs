using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    private void Awake()
    {
        instance = this;
    }

    public Direction SelectRandomDirection()
    {
        return (Direction)Random.Range(1, 5);
    }
}

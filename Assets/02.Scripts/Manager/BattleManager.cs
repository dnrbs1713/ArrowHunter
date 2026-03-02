using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private void Awake()
    {
        instance = this;
    }

    public BattleResult Resolve(Direction attacker, Direction defender)
    {
        bool success = BattleResolver.Resolve(attacker, defender);
        return new BattleResult(success);
    }
}

using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class BattleResolver : MonoBehaviour
{
    public static bool Resolve(Direction attacker, Direction defender)
    {
        Debug.Log($"Attacker: {attacker} / Defender: {defender}");

        return attacker != defender;
    }
}

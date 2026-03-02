using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class PlayerCombatController : MonoBehaviour
{
    public static PlayerCombatController instance;


    private void Awake()
    {
        instance = this;
    }
    public Direction GetDirectionInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) return Direction.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) return Direction.Down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return Direction.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return Direction.Right;

        return Direction.None;
    }

}

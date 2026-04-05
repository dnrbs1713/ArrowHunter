using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public class PlayerCombatController : MonoBehaviour
{
    //public static PlayerCombatController instance;
    public BattleEntity playerEntity;

    private void Awake()
    {
        if(playerEntity == null) playerEntity = GetComponent<BattleEntity>();
    }

    private void Start()
    {
        playerEntity.Initialize(playerEntity.statData);
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

using UnityEngine;
using System;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;
public class PlayerItemInstance 
{
    public ItemSO itemSO { get; private set; }
    public string instanceId { get; private set; }
    public int itemQuantity { get; private set; }
    public Direction storedDirection { get; private set; }
    public bool isEquipped { get; set; }
    public PlayerItemInstance(ItemSO so, int quantity = 1)
    {
        itemSO = so;
        instanceId = Guid.NewGuid().ToString("N");
        itemQuantity = Mathf.Max(1, quantity);
        storedDirection = Direction.None;
        isEquipped = false;
    }

    //포션류 아이템
    public bool CanUse()
    {
        return itemQuantity > 0;
    }
    public void SetStoredDirection(Direction direction)
    {
        storedDirection = direction;
    }

    public void IncreaseQuantity(int amount = 1)
    {
        itemQuantity += Mathf.Max(1, amount);
    }

    public void DecreaseQuantity(int amount = 1)
    {
        itemQuantity = Mathf.Max(0, itemQuantity - Mathf.Max(1, amount));
    }
}

using UnityEngine;
using System.Collections.Generic;
public class PlayerInventory
{
    public List<PlayerItemInstance> obtainedItem { get; private set; }
    private PlayerInstance _owner;
    public PlayerInventory(PlayerInstance owner)
    {
        _owner = owner;
        obtainedItem = new List<PlayerItemInstance>();
    }

    public void ObtainItem(ItemSO item)
    {
        if (item == null)
            return;

        //æ∆¿Ã≈€ ºº∆√
        if (IsStackable(item))
        {
            PlayerItemInstance stackableItem = FindStackableItem(item);

            if (stackableItem != null)
            {
                stackableItem.IncreaseQuantity();
                Debug.Log($"æ∆¿Ã≈€ ¡ﬂ√∏ »πµÊ: {stackableItem.itemSO.name} / ºˆ∑Æ: {stackableItem.itemQuantity}");
                return;
            }
        }

        PlayerItemInstance itemInstance = new PlayerItemInstance(item, 1);
        obtainedItem.Add(itemInstance);

        Debug.Log($"æ∆¿Ã≈€ »πµÊ{itemInstance.itemSO.name}");
    }
    public bool HasItem(PlayerItemInstance itemInstance)
    {
        return itemInstance != null && obtainedItem.Contains(itemInstance);
    }


    public void DeleteItem(PlayerItemInstance itemInstance)
    {
        if (itemInstance == null)
            return;

        if (!HasItem(itemInstance))
            return;

        if (itemInstance.isEquipped)
            _owner.RequestUnequipItem(itemInstance);

        itemInstance.DecreaseQuantity();

        if (itemInstance.itemQuantity == 0)
            obtainedItem.Remove(itemInstance);
    }

    private bool IsStackable(ItemSO item)
    {
        return item.itemType == ItemType.Potion;
    }

    private PlayerItemInstance FindStackableItem(ItemSO item)
    {
        for (int i = 0; i < obtainedItem.Count; i++)
        {
            if (obtainedItem[i].itemSO == item)
                return obtainedItem[i];
        }

        return null;
    }
}

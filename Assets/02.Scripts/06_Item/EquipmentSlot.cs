using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EquipmentSlot
{
    public Dictionary<ItemType, PlayerItemInstance> equippedItem { get; private set; }
    private PlayerInstance _owner;

    public EquipmentSlot(PlayerInstance owner)
    {
        _owner = owner;
        equippedItem = new Dictionary<ItemType, PlayerItemInstance>();
    }
    public bool EquipItem(PlayerItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.itemSO == null)
            return false;

        ItemType type = itemInstance.itemSO.itemType;


        if (!itemInstance.itemSO.IsEquippable)
            return false;

        if (equippedItem.TryGetValue(type, out PlayerItemInstance currentItem))
        {
            if (currentItem == itemInstance)
                return true;

            UnequipItem(currentItem);
        }

        if (equippedItem.ContainsKey(type)){
            UnequipItem(equippedItem[type]);
        }

        equippedItem[type] = itemInstance;
        itemInstance.isEquipped = true;
        ApplyEffects(itemInstance);

        return true;
    }

    public bool UnequipItem(PlayerItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.itemSO == null)
            return false;

        ItemType type = itemInstance.itemSO.itemType;

        if (!equippedItem.TryGetValue(type, out PlayerItemInstance equippedInstance))
            return false;

        if (equippedInstance != itemInstance)
            return false;

        RemoveEffects(itemInstance);
        itemInstance.isEquipped = false;
        equippedItem.Remove(type);

        return true;
    }

    private void ApplyEffects(PlayerItemInstance itemInstance)
    {
        if (itemInstance.itemSO.itemEffect == null)
            return;

        for (int i = 0; i < itemInstance.itemSO.itemEffect.Count; i++)
        {
            ItemEffectSO effect = itemInstance.itemSO.itemEffect[i];

            if (effect == null)
                continue;

            effect.OnEquip(_owner, itemInstance);
        }
    }

    private void RemoveEffects(PlayerItemInstance itemInstance)
    {
        if (itemInstance.itemSO.itemEffect == null)
            return;

        for (int i = 0; i < itemInstance.itemSO.itemEffect.Count; i++)
        {
            ItemEffectSO effect = itemInstance.itemSO.itemEffect[i];

            if (effect == null)
                continue;

            effect.OnUnequip(_owner, itemInstance);
        }
    }

    //아이템 효과 적용
    public void DispatchBattleEvent(ItemBattleEventContext context)
    {
        foreach (var pair in equippedItem)
        {
            PlayerItemInstance itemInstance = pair.Value; // 딕셔너리에서 Value 가져오기

            if (itemInstance == null || itemInstance.itemSO == null)
                continue;

            if (itemInstance.itemSO.itemEffect == null)
                continue;

            // 아이템 효과 발동
            for (int i = 0; i < itemInstance.itemSO.itemEffect.Count; i++)
            {
                ItemEffectSO effect = itemInstance.itemSO.itemEffect[i];

                if (effect == null)
                    continue;

                context.itemSource = itemInstance;
                effect.HandleBattleEvent(context);
            }
        }
    }
}

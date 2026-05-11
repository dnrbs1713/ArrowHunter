using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    public string Rarity;
    public Sprite icon;
    [TextArea]public string description;

    public int itemId;
    public int setId;
    public List<ItemEffectSO> itemEffect;
    public ItemType itemType;
    public StatType itemStat;

    public bool IsEquippable => itemType != ItemType.Potion;
}

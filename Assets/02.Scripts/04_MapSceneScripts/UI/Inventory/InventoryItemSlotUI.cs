using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text equippedMarkText;

    private PlayerItemInstance _item;
    private Action<PlayerItemInstance> _onClick;

    public void Set(PlayerItemInstance item, Action<PlayerItemInstance> onClick)
    {
        _item = item;
        _onClick = onClick;

        if (_item == null || _item.itemSO == null)
            return;

        if (icon != null)
        {
            icon.sprite = _item.itemSO.icon;
            icon.enabled = _item.itemSO.icon != null;
        }

        nameText.text = _item.itemSO.itemName;
        quantityText.text = $"x{_item.itemQuantity}";
        equippedMarkText.text = _item.isEquipped ? "ÀåÂøÁß" : "";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(_item));
    }
}

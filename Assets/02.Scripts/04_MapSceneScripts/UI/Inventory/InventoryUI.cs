using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject rootPanel;

    [Header("List")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private InventoryItemSlotUI itemSlotPrefab;

    [Header("Detail")]
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TMP_Text selectedItemNameText;
    [SerializeField] private TMP_Text selectedItemDescriptionText;
    [SerializeField] private TMP_Text selectedItemTypeText;
    [SerializeField] private TMP_Text selectedItemQuantityText;

    [Header("Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button closeButton;

    [Header("Equipped Icons")]
    [SerializeField] private Image weaponItemIcon;
    [SerializeField] private Image helmetItemIcon;
    [SerializeField] private Image armorItemIcon;
    [SerializeField] private Image glovesItemIcon;
    [SerializeField] private Image pantsItemIcon;
    [SerializeField] private Image shoesItemIcon;
    [SerializeField] private Image pendantItemIcon;
    [SerializeField] private Image ringItemIcon;

    private PlayerInstance _player;
    private PlayerItemInstance _selectedItem;

    private void Awake()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (equipButton != null)
            equipButton.onClick.AddListener(OnClickEquipButton);

        if (unequipButton != null)
            unequipButton.onClick.AddListener(OnClickUnequipButton);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventory);
    }

    public void Show(PlayerInstance player)
    {
        _player = player;

        if (_player == null)
            return;

        if (rootPanel != null)
            rootPanel.SetActive(true);

        RefreshAll();
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    public void OpenInventory()
    {
        if (BattleDataManager.instance == null)
            return;

        PlayerInstance player = BattleDataManager.instance.PlayerInstance;

        if (player == null)
            return;

        Show(player);
    }

    public void CloseInventory()
    {
        Hide();
    }

    private void RefreshAll()
    {
        RefreshSelectionState();
        RefreshItemList();
        RefreshDetail();
        RefreshButtons();
        RefreshEquippedSlots();
    }

    private void RefreshSelectionState()
    {
        if (_player == null)
        {
            _selectedItem = null;
            return;
        }

        if (_selectedItem != null && !_player.playerInventory.HasItem(_selectedItem))
            _selectedItem = null;

        List<PlayerItemInstance> items = _player.playerInventory.obtainedItem;

        if (_selectedItem == null && items.Count > 0)
            _selectedItem = items[0];
    }

    private void RefreshItemList()
    {
        ClearItemList();

        if (_player == null)
            return;

        List<PlayerItemInstance> items = _player.playerInventory.obtainedItem;

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItemSlotUI slotUI = Instantiate(itemSlotPrefab, contentRoot);
            slotUI.Set(items[i], OnClickItemSlot);
        }
    }

    private void ClearItemList()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);
    }

    private void OnClickItemSlot(PlayerItemInstance item)
    {
        _selectedItem = item;
        RefreshDetail();
        RefreshButtons();
    }

    private void RefreshDetail()
    {
        if (_selectedItem == null || _selectedItem.itemSO == null)
        {
            if (selectedItemIcon != null)
            {
                selectedItemIcon.sprite = null;
                selectedItemIcon.enabled = false;
            }

            selectedItemNameText.text = "";
            selectedItemDescriptionText.text = "";
            selectedItemTypeText.text = "";
            selectedItemQuantityText.text = "";
            return;
        }

        ItemSO item = _selectedItem.itemSO;

        if (selectedItemIcon != null)
        {
            selectedItemIcon.sprite = item.icon;
            selectedItemIcon.enabled = item.icon != null;
        }

        selectedItemNameText.text = item.itemName;
        selectedItemDescriptionText.text = item.description;
        selectedItemTypeText.text = item.itemType.ToString();
        selectedItemQuantityText.text = $"x{_selectedItem.itemQuantity}";
    }

    private void RefreshButtons()
    {
        if (_selectedItem == null || _selectedItem.itemSO == null)
        {
            equipButton.interactable = false;
            unequipButton.interactable = false;
            return;
        }

        bool isEquippable = _selectedItem.itemSO.IsEquippable;
        bool isEquipped = _selectedItem.isEquipped;

        equipButton.interactable = isEquippable && !isEquipped;
        unequipButton.interactable = isEquippable && isEquipped;
    }

    private void RefreshEquippedSlots()
    {
        RefreshEquippedSlotIcon(ItemType.Weapon, weaponItemIcon);
        RefreshEquippedSlotIcon(ItemType.Helmet, helmetItemIcon);
        RefreshEquippedSlotIcon(ItemType.Armor, armorItemIcon);
        RefreshEquippedSlotIcon(ItemType.Gloves, glovesItemIcon);
        RefreshEquippedSlotIcon(ItemType.Pants, pantsItemIcon);
        RefreshEquippedSlotIcon(ItemType.Shoes, shoesItemIcon);
        RefreshEquippedSlotIcon(ItemType.Pendant, pendantItemIcon);
        RefreshEquippedSlotIcon(ItemType.Ring, ringItemIcon);
    }

    private void RefreshEquippedSlotIcon(ItemType type, Image slotIcon)
    {
        if (slotIcon == null)
            return;

        if (_player == null)
        {
            slotIcon.sprite = null;
            slotIcon.enabled = false;
            return;
        }

        if (_player.equipmentSlot.equippedItem.TryGetValue(type, out PlayerItemInstance item))
        {
            slotIcon.sprite = item.itemSO.icon;
            slotIcon.enabled = item.itemSO.icon != null;
        }
        else
        {
            slotIcon.sprite = null;
            slotIcon.enabled = false;
        }
    }

    private void OnClickEquipButton()
    {
        if (_player == null || _selectedItem == null)
            return;

        _player.RequestEquipItem(_selectedItem);
        RefreshAll();
    }

    private void OnClickUnequipButton()
    {
        if (_player == null || _selectedItem == null)
            return;

        _player.RequestUnequipItem(_selectedItem);
        RefreshAll();
    }
}

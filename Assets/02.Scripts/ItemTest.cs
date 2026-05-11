using System.Text;
using UnityEngine;
using ArrowClash.Common;
public class ItemTest : MonoBehaviour
{
    [Header("Test Items")]
    [SerializeField] private ItemSO woodSword;
    [SerializeField] private ItemSO manaNecklace;
    [SerializeField] private ItemSO mossSword;

    private PlayerInstance Player
    {
        get
        {
            if (BattleDataManager.instance == null)
                return null;

            return BattleDataManager.instance.PlayerInstance;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            GiveItem(woodSword);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            GiveItem(manaNecklace);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            GiveItem(mossSword);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            PrintInventory();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            EquipFirstUnequippedItem();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            UnequipFirstEquippedItem();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            PrintPlayerAttackInfo();
    }

    private void GiveItem(ItemSO item)
    {
        if (Player == null)
        {
            Debug.LogWarning("[ItemDebugTester] PlayerInstance가 없습니다.");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("[ItemDebugTester] Inspector에 ItemSO가 연결되지 않았습니다.");
            return;
        }

        Player.GetItem(item);
        Debug.Log($"[ItemDebugTester] 아이템 획득: {item.itemName}");
    }

    private void PrintInventory()
    {
        if (Player == null)
        {
            Debug.LogWarning("[ItemDebugTester] PlayerInstance가 없습니다.");
            return;
        }

        var inventory = Player.playerInventory.obtainedItem;

        if (inventory == null || inventory.Count == 0)
        {
            Debug.Log("[ItemDebugTester] 인벤토리가 비어 있습니다.");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[ItemDebugTester] 현재 인벤토리");

        for (int i = 0; i < inventory.Count; i++)
        {
            PlayerItemInstance item = inventory[i];

            sb.AppendLine(
                $"{i}. {item.itemSO.itemName} / 수량: {item.itemQuantity} / 장착중: {item.isEquipped}");
        }

        Debug.Log(sb.ToString());
    }

    private void EquipFirstUnequippedItem()
    {
        if (Player == null)
        {
            Debug.LogWarning("[ItemDebugTester] PlayerInstance가 없습니다.");
            return;
        }

        var inventory = Player.playerInventory.obtainedItem;

        for (int i = 0; i < inventory.Count; i++)
        {
            PlayerItemInstance item = inventory[i];

            if (item == null || item.itemSO == null)
                continue;

            if (item.isEquipped)
                continue;

            if (item.itemSO.itemType == ItemType.Potion)
                continue;

            Player.RequestEquipItem(item);
            Debug.Log($"[ItemDebugTester] 장착 시도: {item.itemSO.itemName}");
            return;
        }

        Debug.Log("[ItemDebugTester] 장착할 수 있는 아이템이 없습니다.");
    }

    private void UnequipFirstEquippedItem()
    {
        if (Player == null)
        {
            Debug.LogWarning("[ItemDebugTester] PlayerInstance가 없습니다.");
            return;
        }

        PlayerItemInstance target = null;

        foreach (var pair in Player.equipmentSlot.equippedItem)
        {
            target = pair.Value;
            break;
        }

        if (target == null)
        {
            Debug.Log("[ItemDebugTester] 장착 중인 아이템이 없습니다.");
            return;
        }

        Player.RequestUnequipItem(target);
        Debug.Log($"[ItemDebugTester] 해제 시도: {target.itemSO.itemName}");
    }

    private void PrintPlayerAttackInfo()
    {
        if (Player == null)
        {
            Debug.LogWarning("[ItemDebugTester] PlayerInstance가 없습니다.");
            return;
        }

        Debug.Log(
            $"[ItemDebugTester] 공격력 확인 / Up:{Player.GetAttack(Direction.Up)} Down:{Player.GetAttack(Direction.Down)} Left:{Player.GetAttack(Direction.Left)} Right:{Player.GetAttack(Direction.Right)}");
    }
}

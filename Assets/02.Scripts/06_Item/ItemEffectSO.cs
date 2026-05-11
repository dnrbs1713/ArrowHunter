using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectSO", menuName = "Scriptable Objects/ItemEffectSO")]
public class ItemEffectSO : ScriptableObject
{
    public virtual void OnEquip(PlayerInstance player, PlayerItemInstance source) { }
    public virtual void OnUnequip(PlayerInstance player, PlayerItemInstance source) { }

    public virtual void HandleBattleEvent(ItemBattleEventContext context) { }
}

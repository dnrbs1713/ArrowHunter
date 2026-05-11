using System;
using UnityEngine;
using static Unity.Cinemachine.CinemachineFreeLookModifier;

[CreateAssetMenu(fileName = "StatModifierItemEffectSO", menuName = "Scriptable Objects/StatModifierItemEffectSO")]
public class StatModifierItemEffectSO : ItemEffectSO
{

    public float value;
    public StatType statType;
    public ModifierMode modifierMode;

    public override void OnEquip(PlayerInstance player, PlayerItemInstance source)
    {
        if (player == null || source == null)
            return;

        StatModifier modifier = new StatModifier(
            statType,
            modifierMode,
            value,
            -1,
            GetSourceId(source)
        );
        player.AddModifier(modifier);
    }

    public override void OnUnequip(PlayerInstance player, PlayerItemInstance source)
    {
        if (player == null || source == null)
            return;

        player.RemoveModifiersBySource(GetSourceId(source));
    }

    private string GetSourceId(PlayerItemInstance source)
    {
        return $"item:{source.instanceId}";
    }
}

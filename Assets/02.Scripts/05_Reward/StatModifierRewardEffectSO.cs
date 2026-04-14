using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "StatModifierRewardEffectSO", menuName = "Scriptable Objects/StatModifierRewardEffectSO")]
public class StatModifierRewardEffectSO : RewardEffectSO
{
    public List<StatModifier> modifiers;

    public override void Apply(PlayerInstance player, RewardSO source)
    {
        foreach(var modifier in modifiers)
        {
            modifier.sourceId = source.name;
            player.AddModifier(modifier);
        }
    }
}

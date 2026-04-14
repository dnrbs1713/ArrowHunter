using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RewardSO", menuName = "Scriptable Objects/RewardSO")]
public class RewardSO : ScriptableObject
{
    public string rewardName;

    [TextArea]
    public string description;

    public Sprite icon;

    public RewardRarity rarity;

    public List<RewardEffectSO> effects;

    public void Apply(PlayerInstance player)
    {
        foreach(var effect in effects)
            effect.Apply(player,this);
    }

}

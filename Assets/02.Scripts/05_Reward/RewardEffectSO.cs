using UnityEngine;

[CreateAssetMenu(fileName = "RewardEffectSO", menuName = "Scriptable Objects/RewardEffectSO")]
public abstract class RewardEffectSO : ScriptableObject
{
    public abstract void Apply(PlayerInstance player, RewardSO source);
}

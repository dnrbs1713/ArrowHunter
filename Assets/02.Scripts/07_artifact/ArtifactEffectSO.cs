using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactEffectSO", menuName = "Scriptable Objects/ArtifactEffectSO")]
public class ArtifactEffectSO : ScriptableObject
{
    public virtual void Apply(ArtifactBattleEventContext context) { }

    public virtual void Delete() { }

    public virtual void HandleBattleEvent(ArtifactBattleEventContext context) { }

}

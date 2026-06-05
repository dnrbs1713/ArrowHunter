using UnityEngine;
using System.Collections.Generic;

public class PlayerArtifactCollection
{
    public List<PlayerArtifactInstance> obtainedArtifact { get; private set; }
    public PlayerInstance _owner;

    public PlayerArtifactCollection(PlayerInstance owner)
    {
        _owner = owner;
        obtainedArtifact = new List<PlayerArtifactInstance>();
    }

    public bool ObtainArtifact(PlayerArtifactInstance artifactInstance)
    {
        if (artifactInstance == null || artifactInstance.artifactSO == null)
            return false;

        if (!HasArtifact(artifactInstance))
            obtainedArtifact.Add(artifactInstance);



        Debug.Log($"¾ÆÆ¼ÆÑÆ® È¹µæ{artifactInstance.artifactSO.artifactName}");

        return true;
    }

    public bool DeleteArtifact(PlayerArtifactInstance artifactInstance)
    {
        if (artifactInstance == null || artifactInstance.artifactSO == null)
            return false;

        if (HasArtifact(artifactInstance))
            obtainedArtifact.Remove(artifactInstance);
        return true;
    }

    public bool HasArtifact(PlayerArtifactInstance artifactInstance)
    {
        foreach (var artifact in obtainedArtifact)
        {
            if (artifact == artifactInstance)
                return true;
        }
        return false;
    }

    public void DispatchBattleEvent(ArtifactBattleEventContext context)
    {
        foreach (var artifact in obtainedArtifact)
        {
            if (artifact == null || artifact.artifactSO == null)
                continue;

            //À¯¹° È¿°ú ¹ßµ¿
            for (int i = 0; i < artifact.artifactSO.artifactEffect.Count; i++)
            {
                ArtifactEffectSO effect = artifact.artifactSO.artifactEffect[i];

                if (effect == null)
                    continue;

                context.artifactInstance = artifact;
                effect.HandleBattleEvent(context);
            }
        }
    }
}

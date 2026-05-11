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

    public void ObtainArtifact(ArtifactSO artifact)
    {
        if (artifact == null)
            return;

        PlayerArtifactInstance artifactInstance = new PlayerArtifactInstance(artifact);
        obtainedArtifact.Add(artifactInstance);

        Debug.Log($"¾ÆÆ¼ÆÑÆ® È¹µæ{artifactInstance.artifactSO.artifactName}");
    }

    public void DeleteItem(PlayerArtifactInstance artifactInstance)
    {
        if (artifactInstance == null)
            return;
        
        if (!HasArtifact(artifactInstance))
            return;

        obtainedArtifact.Remove(artifactInstance);
    }

    public bool HasArtifact(PlayerArtifactInstance artifactInstance)
    {
        return artifactInstance != null && obtainedArtifact.Contains(artifactInstance);
    }
}

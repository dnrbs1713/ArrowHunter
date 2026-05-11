using UnityEngine;
using System.Collections.Generic;

public class ArtifactSlot
{
    public List<PlayerArtifactInstance> playerArtifactList { get; private set; }
    private PlayerInstance _owner;

    public ArtifactSlot(PlayerInstance owner)
    {
        _owner = owner;
        playerArtifactList = new List<PlayerArtifactInstance>();
    }

    public bool GetArtifact(PlayerArtifactInstance artifactInstance)
    {
        if(artifactInstance == null || artifactInstance.artifactSO == null)
            return false;

        if(!HasArtifact(artifactInstance))
            playerArtifactList.Add(artifactInstance );
        return true;
    }

    public bool DeleteArtifact(PlayerArtifactInstance artifactInstance)
    {
        if (artifactInstance == null || artifactInstance.artifactSO == null)
            return false;

        if(HasArtifact(artifactInstance))
            playerArtifactList.Remove(artifactInstance );
        return true;
    }

    public bool HasArtifact(PlayerArtifactInstance artifactInstance)
    {
        foreach(var artifact in playerArtifactList)
        {
            if (artifact == artifactInstance)
                return true;
        }
        return false;
    }

    public void DispatchBattleEvent(ArtifactBattleEventContext context)
    {
        foreach (var artifact in playerArtifactList)
        {
            if (artifact == null || artifact.artifactSO == null)
                continue;

            //유물 효과 발동
            for(int i = 0; i < artifact.artifactSO.artifactEffect.Count; i++)
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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ArrowClash.Common;

public class ArtifactRuntimeState
{
    public int stack;
    public bool attackedThisTurn;
    public bool defenseThisTurn;
    public bool skillUseThisTurn;
}

public class PlayerArtifactInstance
{
    public ArtifactSO artifactSO { get; private set; }
    public Direction storedDirection { get; private set; }
    public bool isEquipped { get; private set; }

    // Effect 관련 변수들
    public ArtifactRuntimeState runtimeState { get; set; }

    public PlayerArtifactInstance(ArtifactSO artifactSO)
    {
        this.artifactSO = artifactSO;
        this.storedDirection = Direction.None;
        this.isEquipped = false;
        this.runtimeState = new ArtifactRuntimeState();
    }

    public void SetStoredDirection(Direction direction)
    {
        storedDirection = direction;
    }
}

using UnityEngine;
using ArrowClash.Common;
public class ArtifactBattleEventContext
{
    public ArtifactEventType eventType;

    public BattleEntity owner;
    public PlayerArtifactInstance artifactInstance;

    public DamageContext damageContext;

    public int iValue;
    public float fValue;

    public Direction direction;
    public StatusType? statusType;
}

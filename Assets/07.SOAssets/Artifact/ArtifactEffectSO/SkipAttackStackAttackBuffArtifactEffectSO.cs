using UnityEngine;

[CreateAssetMenu(fileName = "SkipAttackStackAttackBuffArtifactEffectSO", menuName = "Scriptable Objects/SkipAttackStackAttackBuffArtifactEffectSO")]
public class SkipAttackStackAttackBuffArtifactEffectSO : ArtifactEffectSO
{
    [SerializeField] private int maxStack = 4;
    [SerializeField] private float value = 0.1f;

    public override void HandleBattleEvent(ArtifactBattleEventContext context)
    {
        if (context == null || context.artifactInstance == null)
            return;

        ArtifactRuntimeState state = context.artifactInstance.runtimeState;

        if (state == null)
            return;

        if (context.eventType == ArtifactEventType.TurnStart)
        {
            state.attackedThisTurn = false;
            return;
        }

        if (context.eventType == ArtifactEventType.AttackResolved)
        {
            state.attackedThisTurn = true;
            return;
        }

        if (context.eventType == ArtifactEventType.TurnEnd)
        {
            if (!state.attackedThisTurn)
                state.stack = Mathf.Min(state.stack + 1, maxStack);

            return;
        }

        if (context.eventType == ArtifactEventType.BeforeDealDamage)
        {
            if (context.damageContext == null)
                return;

            context.damageContext.damage *= 1f + (state.stack * value);
        }

        if (context.eventType == ArtifactEventType.AfterDealDamage)
        {
            state.stack = 0;
            return;
        }
    }
}

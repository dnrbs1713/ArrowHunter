using UnityEngine;
using ArrowClash.Common;
public class StatusEffectInstance
{
    public StatusEffectSO data;
    public BattleEntity source;

    public int remainingDuration;
    public int currentStack;

    public float magnitude;
    public int intValue;
    public Direction storedDirection;

    public StatusEffectInstance(StatusEffectSO so, BattleEntity source)
    {
        data = so;
        this.source = source;

        remainingDuration = so.duration;
        currentStack = 1;

        magnitude = 0f;
        intValue = 0;
        storedDirection = Direction.None;
    }
}

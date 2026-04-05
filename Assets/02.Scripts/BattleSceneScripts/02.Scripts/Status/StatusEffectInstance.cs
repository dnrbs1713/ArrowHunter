using UnityEngine;
using ArrowClash.Common;
public class StatusEffectInstance
{
    public StatusEffectSO data;
    public int remainingDuration;
    public int currentStack;
    public float currentDamageValue;
    //public float currentDamageRatio;
    //public int currentFlatDamage;
    public Direction frozenDir;

    public StatusEffectInstance(StatusEffectSO so)
    {
        data = so;
        remainingDuration = so.duration;
        currentStack = 1;
        currentDamageValue = 0f;
        frozenDir = Direction.None;
    }
}

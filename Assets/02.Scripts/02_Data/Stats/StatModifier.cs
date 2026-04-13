using System;
using UnityEngine;

public enum ModifierMode
{
    Flat,
    Percent
}

[Serializable]
public class StatModifier
{
    public StatType statType;
    public ModifierMode mode;
    public float value;

    // -1이면 영구, 1 이상이면 턴 수
    public int duration;

    // 보상 / 버프 / 디버프 출처 추적용
    public string sourceId;

    public bool IsPermanent => duration < 0;
    public bool IsExpired => duration == 0;
    public StatModifier(StatType statType, ModifierMode mode, float value,
        int duration = -1, string sourceId = "")
    {
        this.statType = statType;
        this.mode = mode;
        this.value = value;
        this.duration = duration;
        this.sourceId = sourceId;
    }

    public void Tick()
    {
        if (duration > 0)
            duration--;
    }

}

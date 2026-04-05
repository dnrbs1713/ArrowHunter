using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffectSO", menuName = "Scriptable Objects/SkillEffectSO")]
public abstract class SkillEffectSO : ScriptableObject
{
    public abstract void Apply(BattleEntity caster, BattleEntity target);
}


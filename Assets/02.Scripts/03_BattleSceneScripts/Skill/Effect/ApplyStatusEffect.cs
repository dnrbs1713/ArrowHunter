using UnityEngine;

[CreateAssetMenu(fileName = "ApplyStatusEffect", menuName = "Scriptable Objects/ApplyStatusEffect")]
public class ApplyStatusEffect : SkillEffectSO
{
    [Header("상태 이상")]
    public StatusEffectSO statusEffectSO;

    [Header("스킬마다 다르게 설정 / -1이면 SO 기본값 사용")]
    public int overrideDuration = -1;
    public int stackCount = 1;
    public override void Apply(BattleEntity caster, BattleEntity target)
    {
        if (statusEffectSO == null)
        { 
           Debug.LogWarning("[ApplyStatusEffect] statusSO 미연결"); 
           return;
        }
        target.statusHandler.Apply(statusEffectSO, caster, stackCount, overrideDuration);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "ParalyzeEffectSO", menuName = "Scriptable Objects/ParalyzeEffectSO")]
public class ParalyzeEffectSO : StatusEffectSO
{
    [Range(0f, 1f)]
    public float failChance = 0.3f;

    public override DirectionInputResult ModifyDirectionInput(BattleEntity owner, StatusEffectInstance instance, DirectionInputResult input)
    {
        if (input.isFailed)
            return input;

        if (Random.value > failChance)
            return input;

        Debug.Log("<color=yellow>[마비] 입력 실패! 코스트가 소모되고 스킬 입력이 취소됩니다.</color>");

        return DirectionInputResult.Fail(
            "Paralyze",
            spendCost: true,
            cancelSkillBuffer: true,
            endTurn: false
        );
    }
}

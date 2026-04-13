using UnityEngine;
using ArrowClash.Common;

[CreateAssetMenu(fileName = "FreezeEffectSO", menuName = "Scriptable Objects/FreezeEffectSO")]
public class FreezeEffectSO : StatusEffectSO
{
    public override void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance)
    {
        Direction[] dirs =
        {
            Direction.Up,
            Direction.Down,
            Direction.Left,
            Direction.Right
        };

        instance.storedDirection = dirs[Random.Range(0, dirs.Length)];

        Debug.Log($"<color=cyan>[동결] {owner.statData.jobName}의 {instance.storedDirection} 방향 봉인</color>");
    }

    public override DirectionInputResult ModifyDirectionInput(
        BattleEntity owner,
        StatusEffectInstance instance,
        DirectionInputResult input)
    {
        if (input.isFailed)
            return input;

        if (input.direction != instance.storedDirection)
            return input;

        Debug.Log($"<color=cyan>[동결] {instance.storedDirection} 방향은 봉인되어 입력이 무효화됩니다.</color>");

        return DirectionInputResult.Fail(
            "Freeze",
            spendCost: false,
            cancelSkillBuffer: false,
            endTurn: false
        );
    }
}

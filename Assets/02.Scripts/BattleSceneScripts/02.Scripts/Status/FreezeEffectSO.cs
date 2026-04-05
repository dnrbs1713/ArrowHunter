using UnityEngine;
using ArrowClash.Common;

[CreateAssetMenu(fileName = "FreezeEffectSO", menuName = "Scriptable Objects/FreezeEffectSO")]
public class FreezeEffectSO : StatusEffectSO, IDirectionFilter
{
    // Inspector: durationRule: Ignore / stackRule: Ignore / maxStack: 1
    //            isActionDisable: ✅

    public override void OnApply(BattleEntity owner, BattleEntity attacker, StatusEffectInstance instance)
    {
        Direction[] dirs = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        instance.frozenDir = dirs[UnityEngine.Random.Range(0, dirs.Length)];
        Debug.Log($"<color=cyan>[빙결] {instance.frozenDir} 방향 봉인</color>");
    }

    // IDirectionFilter 구현 — 봉인된 방향이면 None 반환
    public Direction FilterDirection(Direction dir, StatusEffectInstance instance)
    {
        if (instance.frozenDir == dir) return Direction.None;
        return dir;
    }
}
using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using ArrowClash.Common;

public static class BattleResolver 
{
    public static BattleResult Resolve(Direction attacker, Direction defender)
    {
        Debug.Log($"[BattleResolver] Attacker: {attacker} / Defender: {defender}");

        // 공격 방향과 방어 방향이 다르면 공격 성공(true)
        bool isSuccess = (attacker != defender);

        // 결과를 담은 객체를 생성해서 반환합니다.
        return new BattleResult(isSuccess);
    }
}

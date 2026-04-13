using UnityEngine;
using ArrowClash.Common;

public class DirectionInputResult
{
    public Direction direction;
    public bool isFailed;
    public bool spendCostOnFail;
    public bool cancelSkillBuffer;
    public bool endTurnOnFail;
    public string failReason;

    public static DirectionInputResult Success(Direction direction)
    {
        return new DirectionInputResult
        {
            direction = direction,
            isFailed = false,
            spendCostOnFail = false,
            cancelSkillBuffer = false,
            endTurnOnFail = false,
            failReason = ""
        };
    }

    public static DirectionInputResult Fail(string reason, bool spendCost, bool cancelSkillBuffer, bool endTurn)
    {
        return new DirectionInputResult
        {
            direction = Direction.None,
            isFailed = true,
            spendCostOnFail = spendCost,
            cancelSkillBuffer = cancelSkillBuffer,
            endTurnOnFail = endTurn,
            failReason = reason
        };
    }
}

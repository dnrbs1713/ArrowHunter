using UnityEngine;
using ArrowClash.Common;

public enum DamageType
{
    BasicAttack,
    Skill,
    Status,
    TrueDamage
}

public enum DamagePhase
{
    BeforeDefense,
    Defense,
    AfterDefense,
    Final
}
public class DamageContext
{
    public BattleEntity source;
    public BattleEntity target;

    public DamageType damageType;
    public StatusType? statusType;

    public Direction direction;
    public bool isCombo;

    public float damage;
    public float defensePower;
    public DamagePhase phase;

    public bool useDefense = true;
    public bool useOutgoingModifiers = true;
    public bool useIncomingModifiers = true;
    public bool useDamageTakenMultiplier = true;
}

using ArrowClash.Common;

public class ItemBattleEventContext
{
    public ItemBattleEventType eventType;

    public BattleEntity owner;
    public PlayerItemInstance itemSource;

    public DamageContext damageContext;

    public int intValue;
    public Direction direction;
    public StatusType? statusType;
}
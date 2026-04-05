using ArrowClash.Common;
public interface IDirectionFilter
{
    Direction FilterDirection(Direction dir, StatusEffectInstance instance);
}
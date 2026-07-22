namespace GameObjects;

public class TroopEffectArea
{
    public TroopDetail.EventEffect.EventEffect Effect;

    public EffectAreaKind Kind;

    public override string ToString() => $"{Kind} {Effect.Name}";
}
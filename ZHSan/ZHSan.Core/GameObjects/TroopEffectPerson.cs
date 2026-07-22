namespace GameObjects;

public class TroopEffectPerson
{
    public TroopDetail.EventEffect.EventEffect Effect;

    public Person EffectPerson;

    public override string ToString() => $"{EffectPerson.Name} {Effect.Name}";
}
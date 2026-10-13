using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6360 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.childrenLoyalty += influence.GetIntParam();
        person.childrenLoyaltyRate += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.childrenLoyalty -= influence.GetIntParam();
        person.childrenLoyaltyRate -= influence.GetIntParam2();
    }
}
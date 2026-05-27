using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind100 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.MonthIncrementOfTechniquePoint += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.MonthIncrementOfTechniquePoint -= influence.GetIntParam();
    }
}
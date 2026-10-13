using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6350 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.multipleChildrenRate += influence.GetIntParam();
        person.maxChildren += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.multipleChildrenRate -= influence.GetIntParam();
        person.maxChildren -= influence.GetIntParam2();
    }
}
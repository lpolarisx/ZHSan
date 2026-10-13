using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind5110 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.ChanceOfNoCapture += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.ChanceOfNoCapture -= influence.GetIntParam();
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind583 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.HighLevelInformationOnInvestigate = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.HighLevelInformationOnInvestigate = false;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind593 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.LowerLevelInformationWhileInvestigated = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.LowerLevelInformationWhileInvestigated = false;
    }
}
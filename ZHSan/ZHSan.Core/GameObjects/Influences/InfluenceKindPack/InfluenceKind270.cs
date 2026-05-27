using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind270 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.YesOrNoOfObliqueView = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.YesOrNoOfObliqueView = false;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind271 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.YesOrNoOfObliqueOffence = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.YesOrNoOfObliqueOffence = false;
    }
}
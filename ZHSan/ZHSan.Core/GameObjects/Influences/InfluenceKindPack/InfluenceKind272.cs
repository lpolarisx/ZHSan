using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind272 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        troop.YesOrNoOfObliqueStratagem = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.YesOrNoOfObliqueStratagem = true;
    }
}
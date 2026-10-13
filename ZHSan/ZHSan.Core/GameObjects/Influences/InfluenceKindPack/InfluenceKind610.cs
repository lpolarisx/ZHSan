using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind610 : InfluenceKind
{

    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        if (troop.BaseNoAccidentalInjury)
        {
            troop.NoAccidentalInjury = true;
        }
        troop.BaseNoAccidentalInjury = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.BaseNoAccidentalInjury = false;
    }
}
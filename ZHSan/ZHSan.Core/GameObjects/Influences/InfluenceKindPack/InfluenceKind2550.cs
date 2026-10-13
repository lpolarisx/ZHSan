using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2550 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.StratagemOfMillitaryType[influence.GetIntParam()] += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.StratagemOfMillitaryType[influence.GetIntParam()] -= influence.GetIntParam2();
    }
}
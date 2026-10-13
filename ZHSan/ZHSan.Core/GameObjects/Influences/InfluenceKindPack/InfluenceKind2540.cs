using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2540 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.ViewAreaOfMillitaryType[influence.GetIntParam()] += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.ViewAreaOfMillitaryType[influence.GetIntParam()] -= influence.GetIntParam2();
    }
}
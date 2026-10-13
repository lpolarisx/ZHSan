using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2500 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.CriticalOfMillitaryType[influence.GetIntParam()] += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.CriticalOfMillitaryType[influence.GetIntParam()] -= influence.GetIntParam2();
    }
}
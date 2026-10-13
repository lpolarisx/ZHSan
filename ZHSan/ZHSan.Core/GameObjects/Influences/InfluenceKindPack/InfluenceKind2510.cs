using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2510 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.AntiCriticalOfMillitaryType[influence.GetIntParam()] += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.AntiCriticalOfMillitaryType[influence.GetIntParam()] -= influence.GetIntParam2();
    }
}
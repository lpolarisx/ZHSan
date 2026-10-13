using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2560 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.AntiStratagemOfMillitaryType[influence.GetIntParam()] += influence.GetIntParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.AntiStratagemOfMillitaryType[influence.GetIntParam()] -= influence.GetIntParam2();
    }
}
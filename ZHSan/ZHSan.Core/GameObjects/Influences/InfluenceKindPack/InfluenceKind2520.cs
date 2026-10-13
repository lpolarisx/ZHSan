using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2520 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.ArchitectureDamageOfMillitaryType[influence.GetIntParam()] += influence.GetFloatParam2();
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.ArchitectureDamageOfMillitaryType[influence.GetIntParam()] -= influence.GetFloatParam2();
    }
}
using GameEnums;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2320 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Faction faction)
    {
        faction.LevelOfView = (InformationLevel)((int)faction.LevelOfView + influence.GetIntParam() - 3);
    }

    public override void PurifyInfluenceKind(Influence influence, Faction faction)
    {
        faction.LevelOfView = (InformationLevel)((int)faction.LevelOfView - influence.GetIntParam() - 3);
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind860 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var strategemId = influence.GetIntParam();

        if (!troop.AllowedStrategems.Contains(strategemId))
        {
            troop.AllowedStrategems.Add(strategemId);
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var strategemId = influence.GetIntParam();

        if (troop.AllowedStrategems.Contains(strategemId))
        {
            troop.AllowedStrategems.Remove(strategemId);
        }
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind330 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var stuntId = influence.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllStunts.TryGetValue(stuntId, out var stunt))
        {
            troop.AddStunt(stunt);
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var stuntId = influence.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllStunts.TryGetValue(stuntId, out var stunt))
        {
            troop.RemoveStunt(stunt);
        }
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind330 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var stunt = Session.Current.Scenario.GameCommonData.AllStunts.GetStunt(influence.GetIntParam());

        troop.Stunts.AddStunt(stunt);
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var stunt = Session.Current.Scenario.GameCommonData.AllStunts.GetStunt(influence.GetIntParam());

        troop.Stunts.RemoveStunt(stunt);
    }
}
using System.Runtime.Serialization;
using GameManager;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind320 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var id = influence.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllCombatMethods.TryGetValue(id, out var combatMethod))
        {
            troop.CombatMethods.TryAdd(combatMethod.ID, combatMethod);
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var id = influence.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllCombatMethods.TryGetValue(id, out var combatMethod))
        {
            troop.CombatMethods.Remove(combatMethod.ID);
        }
    }
}
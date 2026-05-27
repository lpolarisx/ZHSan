using System.Runtime.Serialization;
using GameManager;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind320 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var combatMethod = Session.Current.Scenario.GameCommonData.AllCombatMethods.GetCombatMethod(influence.GetIntParam());

        troop.CombatMethods.AddCombatMethod(combatMethod);
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        var combatMethod = Session.Current.Scenario.GameCommonData.AllCombatMethods.GetCombatMethod(influence.GetIntParam());

        troop.CombatMethods.RemoveCombatMethod(combatMethod);
    }
}
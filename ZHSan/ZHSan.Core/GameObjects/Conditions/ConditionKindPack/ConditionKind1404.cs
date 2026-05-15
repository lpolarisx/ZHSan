using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1404 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var arch = Session.Current.Scenario.GetArchitectureByPosition(troop.Position);

        return arch != null && arch.BelongedFaction != troop.BelongedFaction;
    }
}
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind998 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return Session.Current.Scenario.IsPlayer(person.BelongedFactionWithPrincess);
    }

    public override bool CheckConditionKind(Condition condition, Architecture architecture)
    {
        return Session.Current.Scenario.IsPlayer(architecture.BelongedFaction);
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return Session.Current.Scenario.IsPlayer(faction);
    }

    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return Session.Current.Scenario.IsPlayer(troop.BelongedFaction);
    }
}
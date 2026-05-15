using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1247 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return !troop.ContactingWillArchitecture;
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1295 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return troop.WillArchitecture == null || troop.WillArchitecture.Endurance > 0;
    }
}
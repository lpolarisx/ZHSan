using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2165 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.PopulationCeiling > 0 && arch.Population / arch.PopulationCeiling < condition.GetFloatParam();
    }
}
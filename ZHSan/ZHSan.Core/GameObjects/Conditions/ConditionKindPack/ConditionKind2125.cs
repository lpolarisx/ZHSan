using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2125 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.TechnologyCeiling > 0 && arch.Technology / arch.TechnologyCeiling < condition.GetFloatParam();
    }
}
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
    public class ConditionKind440 : ConditionKind
    {
        public override bool CheckConditionKind(Condition condition, Person person)
        {
            return (int)person.ValuationOnGovernment == condition.GetIntParam();
        }
    }
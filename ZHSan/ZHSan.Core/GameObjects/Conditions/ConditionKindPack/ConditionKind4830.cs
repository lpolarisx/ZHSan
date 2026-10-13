using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;
[DataContract]public class ConditionKind4830 : ConditionKind
    {
        public override bool CheckConditionKind(Condition condition, Person person)
        {
            return person.NumberOfChildren - markedPerson.NumberOfChildren >= condition.GetIntParam();
        }
    }
using System.Runtime.Serialization;
using GameManager;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind670 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Age < condition.GetIntParam() || !Session.GlobalVariables.PersonNaturalDeath.Value;
    }
}
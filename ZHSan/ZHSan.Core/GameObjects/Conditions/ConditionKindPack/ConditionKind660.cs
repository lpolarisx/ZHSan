using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind660 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Age >= condition.GetIntParam() || !Session.GlobalVariables.PersonNaturalDeath.Value;
    }
}
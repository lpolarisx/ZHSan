using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind40 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var multiple = influence.GetIntParam() - 1;

        if (multiple > 0)
        {
            person.MultipleOfAgricultureTechniquePoint += multiple;
            person.MultipleOfCommerceTechniquePoint += multiple;
            person.MultipleOfTechnologyTechniquePoint += multiple;
            person.MultipleOfDominationTechniquePoint += multiple;
            person.MultipleOfMoraleTechniquePoint += multiple;
            person.MultipleOfEnduranceTechniquePoint += multiple;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var multiple = influence.GetIntParam() - 1;

        if (multiple > 0)
        {
            person.MultipleOfAgricultureTechniquePoint -= multiple;
            person.MultipleOfCommerceTechniquePoint -= multiple;
            person.MultipleOfTechnologyTechniquePoint -= multiple;
            person.MultipleOfDominationTechniquePoint -= multiple;
            person.MultipleOfMoraleTechniquePoint -= multiple;
            person.MultipleOfEnduranceTechniquePoint -= multiple;
        }
    }
}
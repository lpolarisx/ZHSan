using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3010 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfMonthFood += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfMonthFood -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var foodEnoughRate = arch.IsFoodEnough ? 0 : 0.5;
        var foodAbundantRate = arch.IsFoodAbundant ? 0 : 0.5;
        var foodIncomeEnoughRate = arch.IsFoodIncomeEnough ? 0 : 1000;

        return (1 - Math.Pow((double)arch.Food / arch.FoodCeiling, 0.5) + foodEnoughRate + foodAbundantRate + foodIncomeEnoughRate) * influence.GetIntParam() / 200000.0;
    }
}
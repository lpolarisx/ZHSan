using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3020 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.RateIncrementOfMonthFund += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.RateIncrementOfMonthFund -= influence.GetFloatParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var fundEnoughRate = arch.IsFundEnough ? 0 : 0.5;
        var fundAbundantRate = arch.IsFundAbundant ? 0 : 0.5;
        var fundIncomeEnoughRate = arch.IsFundIncomeEnough ? 0 : 1000;

        return (1 - Math.Pow((double)arch.Fund / arch.FundCeiling, 0.5) + fundEnoughRate + fundAbundantRate + fundIncomeEnoughRate) * (arch.ExpectedFund * influence.GetFloatParam() / 200.0);
    }
}
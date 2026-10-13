using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3260 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.RateIncrementOfPopulationDevelop += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.RateIncrementOfPopulationDevelop -= influence.GetFloatParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        if (!arch.Kind.HasPopulation) return -1;

        return (1 - Math.Pow((double)arch.Population / arch.PopulationCeiling, 0.5)) * (0.001 / arch.PopulationDevelopingRate) * (influence.GetFloatParam() * 10000);
    }
}
using GameDatas;

namespace GameObjects.PersonDetail;

public class PersonGeneratorType : GameObject
{
    public int CommandLo { get; set; }

    public int CommandHi { get; set; }

    public int StrengthLo { get; set; }

    public int StrengthHi { get; set; }

    public int IntelligenceLo { get; set; }

    public int IntelligenceHi { get; set; }

    public int PoliticsLo { get; set; }

    public int PoliticsHi { get; set; }

    public int GlamourLo { get; set; }

    public int GlamourHi { get; set; }

    public int BraveLo { get; set; }

    public int BraveHi { get; set; }

    public int CalmnessLo { get; set; }

    public int CalmnessHi { get; set; }

    public int PersonalLoyaltyLo { get; set; }

    public int PersonalLoyaltyHi { get; set; }

    public int AmbitionLo { get; set; }

    public int AmbitionHi { get; set; }

    public int GenerationChance { get; set; }

    public bool AffectedByRateParameter { get; set; }

    public int TitleChance { get; set; }

    public int GenderFix { get; set; }

    public int CostFund { get; set; }

    public int TypeCount { get; set; }

    public int FactionLimit { get; set; }

    public PersonGeneratorType() {}

    public PersonGeneratorType(PersonGeneratorTypeConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        CommandLo = config.CommandLo;
        CommandHi = config.CommandHi;
        StrengthLo = config.StrengthLo;
        StrengthHi = config.StrengthHi;
        IntelligenceLo = config.IntelligenceLo;
        IntelligenceHi = config.IntelligenceHi;
        PoliticsLo = config.PoliticsLo;
        PoliticsHi = config.PoliticsHi;
        GlamourLo = config.GlamourLo;
        GlamourHi = config.GlamourHi;
        BraveLo = config.BraveLo;
        BraveHi = config.BraveHi;
        CalmnessLo = config.CalmnessLo;
        CalmnessHi = config.CalmnessHi;
        PersonalLoyaltyLo = config.PersonalLoyaltyLo;
        PersonalLoyaltyHi = config.PersonalLoyaltyHi;
        AmbitionLo = config.AmbitionLo;
        AmbitionHi = config.AmbitionHi;
        GenerationChance = config.GenerationChance;
        AffectedByRateParameter = config.AffectedByRateParameter;
        TitleChance = config.TitleChance;
        GenderFix = config.GenderFix;
        CostFund = config.CostFund;
        TypeCount = config.TypeCount;
        FactionLimit = config.FactionLimit;
    }
}
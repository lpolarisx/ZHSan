using GameDatas;

namespace GameObjects;

public class DisasterKind : GameObject
{
    public int MinDuration { get; set; }

    public int MaxDuration { get; set; }

    public int PopulationDamage { get; set; }

    public int TroopDamage { get; set; }

    public int FundDamage { get; set; }

    public int FoodDamage { get; set; }

    public int OfficerDamage { get; set; }

    public int DominationDamage { get; set; }

    public int EnduranceDamage { get; set; }

    public int AgricultureDamage { get; set; }

    public int CommerceDamage { get; set; }

    public int TechnologyDamage { get; set; }

    public int MoraleDamage { get; set; }

    public DisasterKind() {}

    public DisasterKind(DisasterKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        MinDuration = config.MinDuration;
        MaxDuration = config.MaxDuration;
        PopulationDamage = config.PopulationDamage;
        TroopDamage = config.TroopDamage;
        FundDamage = config.FundDamage;
        FoodDamage = config.FoodDamage;
        OfficerDamage = config.OfficerDamage;
        DominationDamage = config.DominationDamage;
        EnduranceDamage = config.EnduranceDamage;
        AgricultureDamage = config.AgricultureDamage;
        CommerceDamage = config.CommerceDamage;
        TechnologyDamage = config.TechnologyDamage;
        MoraleDamage = config.MoraleDamage;
    }
}
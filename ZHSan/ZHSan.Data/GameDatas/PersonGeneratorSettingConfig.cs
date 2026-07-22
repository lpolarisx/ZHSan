namespace GameDatas;

public class PersonGeneratorSettingConfig : BaseConfig
{
    public int FemaleChance { get; set; }

    public int ChildrenFemaleChance { get; set; }

    public int BornLo { get; set; }

    public int BornHi { get; set; }

    public int DebutLo { get; set; }

    public int DebutHi { get; set; }

    public int DieLo { get; set; }

    public int DieHi { get; set; }

    public int DebutAtLeast { get; set; }
}
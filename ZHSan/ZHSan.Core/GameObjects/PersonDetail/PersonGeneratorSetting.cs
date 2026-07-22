using GameDatas;

namespace GameObjects.PersonDetail;

public class PersonGeneratorSetting : GameObject
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

    public PersonGeneratorSetting() {}

    public PersonGeneratorSetting(PersonGeneratorSettingConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        FemaleChance = config.FemaleChance;
        ChildrenFemaleChance = config.ChildrenFemaleChance;
        BornLo = config.BornLo;
        BornHi = config.BornHi;
        DebutLo = config.DebutLo;
        DebutHi = config.DebutHi;
        DebutAtLeast = config.DebutAtLeast;
        DieLo = config.DieLo;
        DieHi = config.DieHi;
    }
}
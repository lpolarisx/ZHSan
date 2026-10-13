using System.Collections.Generic;
using GameDatas;

namespace GameObjects.PersonDetail;

public class BiographyAdjectives : GameObject
{
    public int Strength { get; set; }

    public int Command { get; set; }

    public int Intelligence { get; set; }

    public int Politics { get; set; }

    public int Glamour { get; set; }

    public int Braveness { get; set; }

    public int Calmness { get; set; }

    public int PersonalLoyalty { get; set; }

    public int Ambition { get; set; }

    public bool Male { get; set; }

    public bool Female { get; set; }

    public List<string> Text { get; set; }

    public List<string> SuffixText { get; set; }

    public BiographyAdjectives(BiographyAdjectiveConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Strength = config.Strength;
        Command = config.Command;
        Intelligence = config.Intelligence;
        Politics = config.Politics;
        Glamour = config.Glamour;
        Braveness = config.Braveness;
        Calmness = config.Calmness;
        PersonalLoyalty = config.PersonalLoyalty;
        Ambition = config.Ambition;
        Male = config.Male;
        Female = config.Female;
        Text = config.Text;
        SuffixText = config.SuffixText;
    }
}
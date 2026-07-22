using System.Collections.Generic;

namespace GameDatas;

public class BiographyAdjectiveConfig : BaseConfig
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
}
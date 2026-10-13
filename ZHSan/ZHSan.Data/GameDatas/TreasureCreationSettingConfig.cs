
using System.Collections.Generic;

namespace GameDatas;

public class TreasureCreationSettingConfig : BaseConfig
{
    public List<int> EligibleInfluenceIDs { get; set; } = new();

    public int TreasureGroup { get; set; }

    public int Cost { get; set; }

    public List<int> PicIDs { get; set; } = new();
}
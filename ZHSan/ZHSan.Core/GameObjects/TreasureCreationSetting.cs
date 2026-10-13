using System.Collections.Generic;
using GameDatas;

namespace GameObjects;

public class TreasureCreationSetting : GameObject
{
    public List<int> EligibleInfluenceIDs { get; set; } = new List<int>();

    public int TreasureGroup { get; set; }

    public int Cost { get; set; }

    public List<int> PicIDs { get; set; } = new List<int>();

    public TreasureCreationSetting(TreasureCreationSettingConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        EligibleInfluenceIDs = config.EligibleInfluenceIDs;
        TreasureGroup = config.TreasureGroup;
        Cost = config.Cost;
        PicIDs = config.PicIDs;
    }
}
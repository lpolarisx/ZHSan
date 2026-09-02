using System.Collections.Generic;
using GameDatas;
using GameGlobal;

namespace GameObjects.ArchitectureDetail;

/// <summary>
/// 地区
/// </summary>
public class Region : GameObject
{
    public List<Architecture> Architectures { get; set; } = new();
    public Architecture RegionCore;

    public int RegionCoreID { get; set; }

    public string StatesListString { get; set; }

    public List<State> States { get; set; } = new();

    public Region(RegionConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        RegionCoreID = config.RegionCoreID;
        StatesListString = config.StatesListString;
    }

    public RegionConfig ToConfig()
    {
        return new RegionConfig
        {
            Id = ID,
            Name = Name,
            RegionCoreID = RegionCoreID,
            StatesListString = StatesListString,
        };
    }

    public int GetFactionScale(Faction faction)
    {
        var architectureCount = Architectures.Count;

        if (architectureCount <= 0) return 0;

        int num = 0;
        foreach (var architecture in Architectures)
        {
            if (architecture.BelongedFaction == null || faction == architecture.BelongedFaction)
            {
                num++;
            }
        }
        return num * 100 / architectureCount;
    }

    public int GetSectionScale(Section section)
    {
        var sectionCount = section.ArchitectureCount;

        if (Architectures.Count <= 0 || sectionCount <= 0) return 0;

        int num = 0;
        foreach (Architecture architecture in Architectures)
        {
            if (architecture.BelongedSection == section)
            {
                num++;
            }

            if (num >= sectionCount)
            {
                return 100;
            }
        }

        return num * 100 / sectionCount;
    }

    public string RegionCoreString => RegionCore?.Name ?? "----";

    public string StatesString => StaticMethods.SaveNameToString(States);
}
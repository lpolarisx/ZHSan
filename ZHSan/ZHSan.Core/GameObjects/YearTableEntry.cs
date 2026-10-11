using GameDatas;
using GameManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects;

public class YearTableEntry : GameObject
{
    private const char NameSeparator = '•';

    public GameDate Date { get; set; }

    public string Content { get; set; }

    public string FactionsString { get; set; }

    public string FactionNamesString { get; set; }

    public bool IsGloballyKnown { get; set; }

    public YearTableEntry(YearTableConfig config)
    {
        ID = config.Id;
        Date = new GameDate(config.Date);
        Content = config.Content;
        FactionsString = config.FactionsString ?? "";
        FactionNamesString = config.FactionNamesString;
        IsGloballyKnown = config.IsGloballyKnown;
    }

    public YearTableConfig ToConfig()
    {
        return new YearTableConfig
        {
            Id = ID,
            Date = Date.ToConfig(),
            Content = Content,
            FactionsString = FactionsString,
            FactionNamesString = FactionNamesString,
            IsGloballyKnown = IsGloballyKnown,
        };
    }

    private List<Faction> factions;

    /// <summary>
    /// 仅包含现存势力；已灭亡势力的名称通过FactionName1~5仍可取得
    /// </summary>
    public List<Faction> Factions
    {
        get
        {
            if (factions == null)
            {
                var allFactions = Session.Current.Scenario.Factions;
                factions = new List<Faction>();
                foreach (int id in GetFactionIds())
                {
                    if (allFactions.TryGetValue(id, out var faction) && !factions.Contains(faction))
                    {
                        factions.Add(faction);
                    }
                }
            }

            return factions;
        }
        set
        {
            factions = value;
            FactionsString = null;
        }
    }

    public override string ToString() => Content;

    public string FactionName1 => GetFactionName(0);

    public string FactionName2 => GetFactionName(1);

    public string FactionName3 => GetFactionName(2);

    public string FactionName4 => GetFactionName(3);

    public string FactionName5 => GetFactionName(4);

    /// <summary>
    /// 存档前刷新势力ID与名称，已灭亡的势力保留ID和灭亡前的名称
    /// </summary>
    public void RefreshFactionStrings()
    {
        var ids = GetFactionIds();
        var names = new List<string>();
        for (int i = 0; i < ids.Count; i++)
        {
            names.Add(GetFactionName(i));
        }

        FactionsString = string.Join(" ", ids);
        FactionNamesString = string.Join(NameSeparator, names);
    }

    // 本局新增的条目没有FactionsString，以势力对象为准；读档得到的条目以FactionsString为准（其中可能有已灭亡的势力）
    private List<int> GetFactionIds()
    {
        if (FactionsString == null)
        {
            return factions == null ? new List<int>() : factions.Select(faction => faction.ID).ToList();
        }

        var ids = new List<int>();
        foreach (var idString in FactionsString.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
        {
            if (int.TryParse(idString, out var id))
            {
                ids.Add(id);
            }
        }
        return ids;
    }

    private string GetFactionName(int index)
    {
        var ids = GetFactionIds();
        if (index >= ids.Count) return "";

        int id = ids[index];
        if (Session.Current.Scenario.Factions.TryGetValue(id, out var faction)) return faction.Name;

        // 势力已灭亡：本局内灭亡的仍能从对象上取到名称，否则用存档里记录的名称
        var destroyed = factions?.FirstOrDefault(f => f.ID == id);
        if (destroyed != null) return destroyed.Name;

        var savedNames = (FactionNamesString ?? "").Split(NameSeparator);
        return index < savedNames.Length ? savedNames[index] : "";
    }

    public YearTableEntry(int id, GameDate date, List<Faction> factions, string content, bool isGloballyKnown)
    {
        ID = id;
        Date = new GameDate(date);
        Content = content;
        this.factions = factions;
        IsGloballyKnown = isGloballyKnown;
    }
}
using GameDatas;
using GameGlobal;
using GameManager;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects;

[DataContract]
public class YearTableEntry : GameObject
{
    [DataMember]
    public GameDate Date { get; set; }

    [DataMember]
    public string Content { get; set; }

    [DataMember]
    public string FactionsString { get; set; }

    [DataMember]
    public bool IsGloballyKnown { get; set; }

    public YearTableEntry(YearTableConfig config)
    {
        ID = config.Id;
        Date = new GameDate(config.Date);
        Content = config.Content;
        FactionsString = config.FactionsString;
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
            IsGloballyKnown = IsGloballyKnown,
        };
    }

    private List<Faction> factions;

    public List<Faction> Factions
    {
        get
        {
            if (factions == null)
            {
                factions = StaticMethods.LoadFromString(Session.Current.Scenario.Factions, FactionsString).Values.ToList();
            }

            return factions;
        }
        set
        {
            factions = value;
        }
    }

    public override string ToString() => Content;

    public string FactionName1
    {
        get
        {
            if (factions.Count >= 1) return factions[0].Name;

            return "";
        }
    }

    public string FactionName2
    {
        get
        {
            if (factions.Count >= 2) return factions[1].Name;

            return "";
        }
    }

    public string FactionName3
    {
        get
        {
            if (factions.Count >= 3) return factions[2].Name;

            return "";
        }
    }

    public string FactionName4
    {
        get
        {
            if (factions.Count >= 4) return factions[3].Name;

            return "";
        }
    }

    public string FactionName5
    {
        get
        {
            if (factions.Count >= 5) return factions[4].Name;

            return "";
        }
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
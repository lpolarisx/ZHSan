using System;
using System.Runtime.Serialization;
using GameDatas;
using Tools;

namespace GameManager;

[DataContract]
public class Scenario
{
    [DataMember]
    public string ID { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public string Time { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Create { get; set; }

    [DataMember]
    public string Info { get; set; }

    [DataMember]
    public string First { get; set; }

    [DataMember]
    public string Desc { get; set; }

    [DataMember]
    public string IDs { get; set; }

    [DataMember]
    public string Names { get; set; }

    [DataMember]
    public string Players { get; set; }

    [DataMember]
    public string Player { get; set; }

    [DataMember]
    public string PlayTime { get; set; }

    [DataMember]
    public string LeaderPics { get; set; }

    [DataMember]
    public string LeaderNames { get; set; }

    [DataMember]
    public string Reputations { get; set; }

    [DataMember]
    public string ArchitectureCounts { get; set; }

    [DataMember]
    public string CapitalNames { get; set; }

    [DataMember]
    public string Populations { get; set; }

    [DataMember]
    public string MilitaryCounts { get; set; }

    [DataMember]
    public string Funds { get; set; }

    [DataMember]
    public string Foods { get; set; }

    [DataMember]
    public string Mod { get; set; }

    public Scenario() {}

    public Scenario(ScenarioConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Path = config.Path;
        Time = config.Time;
        Title = config.Title;
        Create = config.Create;
        Info = config.Info;
        First = config.First;
        Desc = config.Desc;
        IDs = config.Ids;
        Names = config.Names;
        Players = config.Player;
        Player = config.Player;
        PlayTime = config.PlayTime;
        LeaderPics = config.LeaderPics;
        LeaderNames = config.LeaderNames;
        Reputations = config.Reputations;
        ArchitectureCounts = config.ArchitectureCounts;
        CapitalNames = config.CapitalNames;
        Populations = config.Populations;
        MilitaryCounts = config.MilitaryCounts;
        Funds = config.Funds;
        Foods = config.Foods;
        Mod = config.Mod;
    }

    public ScenarioConfig ToConfig()
    {
        return new ScenarioConfig
        {
            Id = ID,
            Name = Name,
            Path = Path,
            Time = Time,
            Title = Title,
            Create = Create,
            Info = Info,
            First = First,
            Desc = Desc,
            Ids = IDs,
            Names = Names,
            Players = Players,
            Player = Player,
            PlayTime = PlayTime,
            LeaderPics = LeaderPics,
            LeaderNames = LeaderNames,
            Reputations = Reputations,
            ArchitectureCounts = ArchitectureCounts,
            CapitalNames = CapitalNames,
            Populations = Populations,
            MilitaryCounts = MilitaryCounts,
            Funds = Funds,
            Foods = Foods,
            Mod = Mod,
        };
    }

    private string GameTime
    {
        get
        {
            if (!int.TryParse(PlayTime, out var seconds)) return "";

            var t = TimeSpan.FromSeconds(seconds);

            return $"{(int)t.TotalHours}:{t.Minutes:D2}";
        }
    }

    public string Summary
    {
        get
        {
            string autoSaveTag = ID == "00" ? "(自动保存) " : "";
            string prefix = $"存档{ID}:    ";

            if (string.IsNullOrEmpty(Title))
            {
                return $"{autoSaveTag}{prefix}空 白 存 档";
            }

            return autoSaveTag + string.Join("  ", prefix, Info, Title, Mod, Time.ToSeasonDate(), Create.ToSeasonShortTime(), $"({GameTime})");
        }
    }
}
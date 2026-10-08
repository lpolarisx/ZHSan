
namespace GameDatas;

public class SettingConfig
{
    public string UserGuid { get; set; }

    public string DeviceID { get; set; }

    public string Language { get; set; }

    public int? MusicVolume { get; set; }

    public int? SoundVolume { get; set; }

    public string DisplayMode { get; set; }
    
    public string Resolution { get; set; }

    public string GamerName { get; set; }

    //[DataMember]
    //public string Difficulty { get; set; }//GlobalVariables已有GameDifficulty

    //[DataMember]
    //public string BattleSpeed { get; set; }//GlobalVariables已有FastBattleSpeed

    public int? SpeedUp { get; set; }

    public bool Chuchangsuiji { get; set; }

    public string MOD { get; set; }

    /// <summary>
    /// 头像包
    /// </summary>
    public string PortraitPack { get; set; }

    public GlobalVariablesConfig GlobalVariables { get; set; }
}
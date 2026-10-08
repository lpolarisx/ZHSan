using Tools;
using System;
using System.Runtime.Serialization;
using Platforms;
using GameGlobal;
using GameDatas;

namespace GameManager
{
    [DataContract]
    public class Setting
    {
        [DataMember]
        public string UserGuid { get; set; }
        [DataMember]
        public string DeviceID { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public int? MusicVolume { get; set; }
        [DataMember]
        public int? SoundVolume { get; set; }
        [DataMember]
        public string DisplayMode { get; set; }
        [DataMember]
        public string Resolution { get; set; }
        [DataMember]
        public string GamerName { get; set; }
        //[DataMember]
        //public string Difficulty { get; set; }//GlobalVariables已有GameDifficulty
        //[DataMember]
        //public string BattleSpeed { get; set; }//GlobalVariables已有FastBattleSpeed
        [DataMember]
        public int? SpeedUp { get; set; }
        [DataMember]
        public bool Chuchangsuiji { get; set; }

        [DataMember]
        public string MOD { get; set; }

        /// <summary>
        /// 头像包
        /// </summary>
        [DataMember]
        public string PortraitPack { get; set; }

        public string MODRuntime
        {
            get
            {
                if (Session.Current == null || Session.Current.Scenario == null || Session.Current.Scenario.MOD == null)
                {
                    return Setting.Current.MOD;
                }
                else
                {
                    return Session.Current.Scenario.MOD;
                }
            }
        }

        [DataMember]
        public GlobalVariables GlobalVariables { get; set; }

        public static Setting Current = null;

        public Setting() {}

        public Setting(SettingConfig config)
        {
            UserGuid = config.UserGuid;
            DeviceID = config.DeviceID;
            Language = config.Language;
            MusicVolume = config.MusicVolume;
            SoundVolume = config.SoundVolume;
            DisplayMode = config.DisplayMode;
            Resolution = config.Resolution;
            GamerName = config.GamerName;
            SpeedUp = config.SpeedUp;
            Chuchangsuiji = config.Chuchangsuiji;
            MOD = config.MOD;
            PortraitPack = config.PortraitPack;
            GlobalVariables = new GlobalVariables(config.GlobalVariables);
        }

        public SettingConfig ToConfig()
        {
            return new SettingConfig
            {
                UserGuid = UserGuid,
                DeviceID = DeviceID,
                Language = Language,
                MusicVolume = MusicVolume,
                SoundVolume = SoundVolume,
                DisplayMode = DisplayMode,
                Resolution = Resolution,
                GamerName = GamerName,
                SpeedUp = SpeedUp,
                Chuchangsuiji = Chuchangsuiji,
                MOD = MOD,
                PortraitPack = PortraitPack,
                GlobalVariables = GlobalVariables.ToConfig(),
            };
        }

        public static void Init(bool prepare)
        {
            const string file = "Setting.config";

            try
            {
                if (Platform.Current.UserFileExist(file))
                {
                    Current = SimpleSerializer.DeserializeJsonFile<Setting>(file, true) ?? new Setting();
                }
            }
            catch (Exception ex)
            {
                WebTools.TakeWarnMsg("初始用户设置失败:Setting.config", "Init:", ex);
            }
            
            Current ??= new Setting();

            if (prepare)
            {
                Prepare();
            }

            Save();

            //if (Platform.PlatFormType == PlatFormType.iOS)  //|| Platform.PlatForm == PlatForm.WinRT || Platform.PlatForm == PlatForm.WP)
            //{
            //	Session.RealResolution = Session.Resolution = Platform.PreferResolution;
            //}
        }

        public static void Save()
        {
            string file1 = "Setting.config";

            // string filePath = Platform.Current.GetUserFilePath("Setting.config");
            // var settingStore = new JsonStore<SettingConfig>(filePath);
            // settingStore.Save(Setting.Current);

            SimpleSerializer.SerializeJsonFile<Setting>(Setting.Current, file1);
        }

        static void Prepare()
        {
            if (Current == null) return;

            if (string.IsNullOrEmpty(Current.UserGuid))
            {
                Current.UserGuid = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(Current.DeviceID))
            {
                Current.DeviceID = Platform.Current.GetDeviceID();
            }

            if (string.IsNullOrEmpty(Current.DisplayMode))
            {
                Current.DisplayMode = Platform.Current.PreferFullMode;
            }

            if (Current.MusicVolume == null)
            {
                Current.MusicVolume = 70;
            }

            if (Current.SoundVolume == null)
            {
                Current.SoundVolume = 50;
            }

            if (Current.SpeedUp == null)
            {
                Current.SpeedUp = 6;
            }

            if (string.IsNullOrEmpty(Current.Chuchangsuiji.ToString()))
            {
                Current.Chuchangsuiji = false;
            }

            //if (Current.NewsBoard == null)
            //{
            //    Current.NewsBoard = new NewsBoard() { Detail = "游戏公告加载中，请稍候……" };
            //}

            if (string.IsNullOrEmpty(Current.Language))
            {
                DetectLanguage();
            }

            if (Current.GlobalVariables == null)
            {
                Current.GlobalVariables = Session.globalVariablesBasic.Clone();
            }

            if (string.IsNullOrEmpty(Session.Resolution))  // Season.PlatForm == PlatForm.iOS || Season.PlatForm == PlatForm.WinRT || Season.PlatForm == PlatForm.WP)
            {
                Session.Resolution = Platform.PreferResolution;
            }
            
            Session.RealResolution = Session.Resolution = Setting.Current.Resolution;
        }

        /// <summary>
        /// 检测系统语言
        /// </summary>
        /// <returns></returns>
        private static string DetectLanguage()
        {
            string name = null;
            try
            {
                name = Platform.Current.CurrentLanguage;
            }
            catch
            {
                // 获取系统语言失败,使用默认值
            }

            bool isCn = name != null && name.Contains("cn", StringComparison.OrdinalIgnoreCase);
            return isCn ? "cn" : "tw";
        }
    }
}
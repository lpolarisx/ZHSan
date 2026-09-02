using System;
using System.Collections.Generic;
//using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GameFreeText;
using GameGlobal;
using GameObjects;
using GameObjects.FactionDetail;
using GameObjects.PersonDetail;
using GameObjects.SectionDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PluginInterface;
using WorldOfTheThreeKingdoms.GameLogic;
using WorldOfTheThreeKingdoms.GameScreens;
using WorldOfTheThreeKingdoms.GameScreens.ScreenLayers;
using WorldOfTheThreeKingdoms.Resources;
using Platforms;
using GameManager;
using System.Diagnostics;
using youcelanPlugin;
using GameEnums;

//using GameObjects.PersonDetail.PersonMessages;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    partial class MainGameScreen : Screen
    {
        public void Initialize()
        {

            if (base.LoadScenarioInInitialization)
            {
                //原ACCESS加載方式，用於將MDB轉為json
                //this.LoadScenarioOld(base.InitializationFileName, base.InitializationFactionIDs);

                this.LoadScenario(base.InitializationFileName, base.InitializationFactionIDs, true, this);

                Session.Current.Scenario.MOD = Setting.Current.MOD;

                var globalVariables = Session.globalVariablesTemp;  //.globalVariablesBasic.Clone();

                var gameParameters = Session.parametersTemp;  //.parametersBasic.Clone();

                if (Session.Current.Scenario.GlobalVariables != null)
                {
                    if (Session.Current.Scenario.GlobalVariables.PersonNaturalDeath != null)
                    {
                        bool personNatureDeath = (bool)Session.Current.Scenario.GlobalVariables.PersonNaturalDeath;
                        globalVariables.PersonNaturalDeath = personNatureDeath;
                    }
                }

                if (InitializationFactionIDs.Count == 0)
                {
                    globalVariables.SkyEye = true;
                }
                else
                {
                    globalVariables.SkyEye = false;
                }

                Session.Current.Scenario.GlobalVariables = globalVariables;
                Session.Current.Scenario.Parameters = gameParameters;
                //以下修改是为了可以使剧本自带一些设置，这样可以使剧本作者能够预设一些特殊设定
                /*if (Session.Current.Scenario.GlobalVariables != null)
                {
                    System.Reflection.FieldInfo[] 非getset字段表 = typeof(GlobalVariables).GetFields((System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
                    foreach (var v in 非getset字段表)
                    {
                        if (v.GetValue(Session.Current.Scenario.GlobalVariables) == null)
                        {
                            v.SetValue(Session.Current.Scenario.GlobalVariables, v.GetValue(globalVariables));
                        }
                    }
                }
                else
                {
                    Session.Current.Scenario.GlobalVariables = globalVariables;
                }

                if (Session.Current.Scenario.Parameters != null)
                {
                    System.Reflection.FieldInfo[] 非getset字段表 = typeof(Parameters).GetFields((System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
                    foreach (var v in 非getset字段表)
                    {
                        if (v.GetValue(Session.Current.Scenario.Parameters) == null)
                        {
                            v.SetValue(Session.Current.Scenario.Parameters, v.GetValue(gameParameters));
                        }
                    }
                }
                else
                {
                    Session.Current.Scenario.Parameters = gameParameters;
                }*/

                // Session.Current.Scenario.GlobalVariables = globalVariables;
                // Session.Current.Scenario.Parameters = gameParameters;

                //this.mainMapLayer.jiazaibeijingtupian();
                //Session.Current.Scenario.InitializeScenarioPlayerFactions(base.InitializationFactionIDs);

                if (Session.Current.Scenario.PlayerFactions.Count == 0)
                {
                    oldDialogShowTime = Setting.Current.GlobalVariables.DialogShowTime;
                    Setting.Current.GlobalVariables.DialogShowTime = 0;
                }
                else
                {
                    if (oldDialogShowTime >= 0)
                    {
                        Setting.Current.GlobalVariables.DialogShowTime = oldDialogShowTime;
                    }
                    else
                    {
                        //Setting.Current.GlobalVariables.DialogShowTime = Session.globalVariablesBasic.DialogShowTime;
                    }
                }

                if (Session.Current.Scenario.PlayerFactions.Count > 0)   //开始新游戏
                {
                    foreach (Faction faction in Session.Current.Scenario.PlayerFactions)
                    {
                        if (faction.FirstSection != null)
                        {
                            var sectionAIDetails = CommonData.GetSectionAIDetailsByConditions(
                                SectionOrientationKind.None,
                                autoRun: false,
                                valueOffensiveCampaign: false,
                                allowOffensiveCampaign: false,
                                allowMilitaryTransfer: false,
                                valueRecruitment: false);

                            faction.FirstSection.AIDetail = sectionAIDetails.First();
                        }
                    }
                    foreach (var architecture in Session.Current.Scenario.Architectures.Values)
                    {
                        architecture.youzainan = false;
                        if (Session.Current.Scenario.IsPlayer(architecture.BelongedFaction))
                        {
                            architecture.AutoHiring = true;
                            architecture.AutoRewarding = true;
                        }
                    }
                    /*
                    foreach (Person wujiang in Session.Current.Scenario.Persons)
                    {
                        wujiang.huaiyun = false;
                        wujiang.faxianhuaiyun = false;
                        wujiang.huaiyuntianshu = -1;
                        wujiang.suoshurenwu = -1;
                    }*/

                    Session.Current.Scenario.CurrentPlayer = Session.Current.Scenario.PlayerFactions[0] as Faction;
                }                
            }
            else  //从开始菜单读取游戏
            {
                this.LoadFileName = base.InitializationFileName;

                this.LoadScenario(base.InitializationFileName, null, false, this);

                //this.Plugins.DateRunnerPlugin.Reset();
                //this.Plugins.GameRecordPlugin.Clear();
                //this.Plugins.GameRecordPlugin.RemoveDisableRects();
                //this.Plugins.AirViewPlugin.RemoveDisableRects();                

                //Session.Current.Scenario.EnableLoadAndSave = false;
                //string realPath = fileName.Substring(0, fileName.Length - 4) + ".mdb";
                //if (this.LoadFileName.EndsWith(".zhs"))
                //{
                //    FileEncryptor.DecryptFile(fileName, realPath, Session.GlobalVariables.cryptKey);
                //}
                //if (Session.GlobalVariables.EncryptSave)
                //{
                //    File.Delete(realPath);
                //}

                Session.Current.Scenario.EnableLoadAndSave = true;
            }
            if ((Platform.PlatFormType == PlatFormType.Win || Platform.PlatFormType == PlatFormType.Desktop) && !Session.MainGame.loaded2)
            {
                Session.MainGame.loaded2 = true;
                //首次载入游戏界面结束后，绘制地图之前,改变窗口的位置和大小
                /*
                Session.MainGame.Window.Position = new Point(0, 0);
                Platform.SetGraphicsWidthHeight(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - 50, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 50);
                Platform.GraphicsApplyChanges();
                */
            }
            this.mainMapLayer.Initialize();

            this.Plugins.InitializePlugins(this);

            this.chushihuajianzhubiaotiheqizi();
            //this.ReloadScreenData();

            //this.thisGame.jiazaitishi.jiazaijindu.Value = 10;            
            //this.thisGame.jiazaitishi.jiazaijindu.Value = 20;           
            //this.thisGame.jiazaitishi.jiazaijindu.Value = 40;
            //this.thisGame.jiazaitishi.jiazaijindu.Value = 60;
            //this.thisGame.jiazaitishi.jiazaijindu.Value = 80;

            //base.Initialize();

            //this.thisGame.Player.stop();

            InitEvents();

            if (base.LoadScenarioInInitialization)
            {
                Session.GlobalVariables.SaveToXml();
                Session.Parameters.SaveToXml();
                Setting.Current.GlobalVariables = Session.globalVariablesTemp.Clone();
                Setting.Save();
                //这里已经保存了玩家自定的信息后，再单独加载剧本制作者设定的信息，这样不会将玩家的设定保存覆盖
                string str = @"Content\Data\Scenario\" + base.InitializationFileName + "GlobalVariables.xml";
                if (File.Exists(Environment.CurrentDirectory + "\\" + str))
                {
                    Session.Current.Scenario.GlobalVariables.InitialGlobalVariables(str);
                }
                string str2 = @"Content\Data\Scenario\" + base.InitializationFileName + "GameParameters.xml";
                if (File.Exists(Environment.CurrentDirectory + "\\" + str2))
                {
                    Session.Current.Scenario.Parameters.InitializeGameParameters(str2);
                }

                Session.Current.Scenario.AfterLoadGameScenario(this);
            }
            else
            {
                Session.Current.Scenario.AfterLoadSaveFile(this);
            }

            this.architectureLayer.Initialize();
            this.mapVeilLayer.Initialize(this);
            this.troopLayer.Initialize();
            this.selectingLayer.Initialize(this);
            this.tileAnimationLayer.Initialize();
            this.routewayLayer.Initialize();
            this.screenManager.Initialize();

            JumpToFaction();

            if (Session.Current.Scenario.CurrentPlayer != null)
            {
                this.Showyoucelan(UndoneWorkKind.None, FrameKind.Architecture, FrameFunction.Jump, false, true, false, false, [.. Session.Current.Scenario.CurrentPlayer.FirstSection.Architectures], null, "", "");
                //this.Plugins.youcelanPlugin.IsShowing = true;
                ((this.Plugins.youcelanPlugin as youcelanPlugin.TabListPlugin).TabList as TabListInFrame).SetMouseEvent(this, true);
            }
            Session.Current.Scenario.Date.SetSeason();
            //this.thisGame.jiazaitishi.jiazaijindu.Value = 90;
        }

        private void JumpToFaction()
        {
            if (base.LoadScenarioInInitialization)
            {
                if (Session.Current.Scenario.CurrentPlayer != null)
                {
                    Session.Current.Scenario.runScenarioStart(Session.Current.Scenario.CurrentPlayer.Capital, this);
                    this.JumpTo((Session.Current.Scenario.PlayerFactions[0] as Faction).Leader.Position);        //地图跳到玩家势力的首领处
                }
            }
        }

        private void chushihuajianzhubiaotiheqizi()
        {
            //System.Drawing.Font fontjianzhu = new System.Drawing.Font("华文中宋", 16f);
            Color colorjianzhu = new Color();
            colorjianzhu.PackedValue = uint.Parse("4294967040");

            //System.Drawing.Font font1 = new System.Drawing.Font("方正北魏楷书繁体", 30f);   //方正北魏楷书繁体
            //Microsoft.Xna.Framework.Color color1 = new Color(1f, 1f, 1f);

            //qizidezi = new FreeText(new System.Drawing.Font("方正北魏楷书繁体", 30f), new Color(1f, 1f, 1f));

            foreach (var architecture in Session.Current.Scenario.Architectures.Values)
            {
                //architecture.jianzhubiaoti = new FreeText(fontjianzhu, colorjianzhu);
                ///////architecture.jianzhubiaoti.DisplayOffset = new Point(0, -mainMapLayer.TileWidth / 2);
                //architecture.jianzhubiaoti.Text = architecture.Name;
                //architecture.jianzhubiaoti.Align = TextAlign.Left;
                architecture.jianzhuqizi = new qizi();
                //architecture.jianzhuqizi.qizidezi = new FreeText(font1, color1);

                try
                {
                    architecture.CaptionTexture = CacheManager.GetTempTexture("Content/Textures/Resources/Architecture/Caption/" + architecture.CaptionID + ".png");
                    architecture.CaptionTexture.Width = 120;
                    architecture.CaptionTexture.Height = 28;
                }
                catch
                {
                    architecture.CaptionTexture = CacheManager.GetTempTexture("Content/Textures/Resources/Architecture/Caption/None.png");
                }

                /*
                if (architecture.BelongedFaction != null)
                {
                    architecture.jianzhuqizi.qizidezi.Text = architecture.BelongedFaction.ToString().Substring(0, 1);
                }*/

                //this.qizidezi.Align = TextAlign.Middle;

                architecture.jianzhuqizi.qizipoint = new Point(architecture.dingdian.X, architecture.dingdian.Y - 1);

            }
        }

        public bool LoadAvail()
        {
            return Session.Current.Scenario.LoadAvail();
        }

        public bool SaveAvail()
        {
            return Session.Current.Scenario.SaveAvail();
        }

#pragma warning disable CS0108 // 'MainGameScreen.LoadContent()' hides inherited member 'Screen.LoadContent()'. Use the new keyword if hiding was intended.
        protected void LoadContent()
#pragma warning restore CS0108 // 'MainGameScreen.LoadContent()' hides inherited member 'Screen.LoadContent()'. Use the new keyword if hiding was intended.
        {
            base.LoadContent();
        }

        public override void LoadGame()   //从游戏里读取存档
        {
            this.Plugins.OptionDialogPlugin.SetStyle("SaveAndLoad");
            this.Plugins.OptionDialogPlugin.SetTitle("读取进度");
            this.Plugins.OptionDialogPlugin.Clear();

            var saves = GameScenario.LoadScenarioSaves();
            for (int i = 0; i <= GameScenario.savemaxcounts; i++)
            {
                string ss = i < 10 ? "0" + i.ToString() : i.ToString();
                GameDelegates.VoidFunction voidFunction = delegate
                {
                    var sce = saves[int.Parse(ss)];

                    if (!String.IsNullOrEmpty(sce.Title))
                    {
                        mainMapLayer.StopThreads();
                        Session.StartScenario(sce, true);
                    }
                };
                saves[i].ID = ss;
                this.Plugins.OptionDialogPlugin.AddOption(saves[i].Summary, null, voidFunction);
            }

            this.Plugins.OptionDialogPlugin.EndAddOptions();
            this.Plugins.OptionDialogPlugin.ShowOptionDialog(ShowPosition.Center);
        }
        
        public override void ReloadScreenData()
        {
            //this.mainMapLayer.jiazaibeijingtupian();

            this.chushihuajianzhubiaotiheqizi();
            this.gengxinyoucelan();
        }

        private void LoadGameFromPosition(string id)
        {
            var saves = GameScenario.LoadScenarioSaves();

            var sce = saves[int.Parse(id)];

            if (!String.IsNullOrEmpty(sce.Title))
            {
                mainMapLayer.StopThreads();
                Session.StartScenario(sce, true);
            }
        }

        private void LoadGameFromAutoPosition()
        {
            LoadGameFromPosition("00");
            //this.LoadFileName = "AutoSave" + this.SaveFileExtension;
            //Thread thread = new Thread(new ThreadStart(this.LoadGameFromDisk));
            //thread.Start();
            //thread.Join();
            //thread = null;
        }

        public static GameScenario LoadScenarioData(string scenarioName, bool fromScenario)
        {
            Session.Current.IsWorking = true;

            //bool zip = true;

            //if (Platform.PlatFormType == PlatFormType.Win || Platform.PlatFormType == PlatFormType.Desktop)
            //{
            //    zip = false;
            //}

            var isUserFile = !fromScenario;
            var scenario = Tools.SimpleSerializer.DeserializeJsonFile<GameScenario>(scenarioName, isUserFile);

            if (scenario == null)
            {
                scenario = Tools.SimpleSerializer.DeserializeJsonFile<GameScenario>(scenarioName, true, true);
            }

            Session.Current.IsWorking = false;

            scenario.LoadedFileName = scenarioName;

            scenario.UsingOwnCommonData = true;

            if (scenario.GameCommonData == null)
            {
                scenario.GameCommonData = CommonData.Current;
                scenario.UsingOwnCommonData = false;
            }
            else
            {
                var commonData = scenario.GameCommonData;

                GameScenario.ProcessCommonData(commonData);

                #region 处理CommonData

                if (commonData.AllArchitectureKinds.Count == 0)
                {
                    commonData.AllArchitectureKinds = CommonData.Current.AllArchitectureKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllAttackDefaultKinds.Count == 0)
                {
                    commonData.AllAttackDefaultKinds = CommonData.Current.AllAttackDefaultKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllAttackTargetKinds.Count == 0)
                {
                    commonData.AllAttackTargetKinds = CommonData.Current.AllAttackTargetKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllCastDefaultKinds.Count == 0)
                {
                    commonData.AllCastDefaultKinds = CommonData.Current.AllCastDefaultKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllCastTargetKinds.Count == 0)
                {
                    commonData.AllCastTargetKinds = CommonData.Current.AllCastTargetKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllCharacterKinds.Count == 0)
                {
                    commonData.AllCharacterKinds = CommonData.Current.AllCharacterKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllColors.Count == 0)
                {
                    commonData.AllColors = CommonData.Current.AllColors;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllCombatMethods.Count == 0)
                {
                    commonData.AllCombatMethods = CommonData.Current.AllCombatMethods;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllConditionKinds.Count == 0)
                {
                    commonData.AllConditionKinds = CommonData.Current.AllConditionKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllConditions.Count == 0)
                {
                    commonData.AllConditions = CommonData.Current.AllConditions;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllFacilityKinds.Count == 0)
                {
                    commonData.AllFacilityKinds = CommonData.Current.AllFacilityKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.GroupedFacilityKindLevels.Count == 0)
                {
                    commonData.GroupedFacilityKindLevels = CommonData.Current.GroupedFacilityKindLevels;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllDisasterKinds.Count == 0)
                {
                    commonData.AllDisasterKinds = CommonData.Current.AllDisasterKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllOfficialTitleKinds.Count == 0)
                {
                    commonData.AllOfficialTitleKinds = CommonData.Current.AllOfficialTitleKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllIdealTendencyKinds.Count == 0)
                {
                    commonData.AllIdealTendencyKinds = CommonData.Current.AllIdealTendencyKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllInfluenceKinds.Count == 0)
                {
                    commonData.AllInfluenceKinds = CommonData.Current.AllInfluenceKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllInfluences.Count == 0)
                {
                    commonData.AllInfluences = CommonData.Current.AllInfluences;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllInformationKinds.Count == 0)
                {
                    commonData.AllInformationKinds = CommonData.Current.AllInformationKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllMilitaryKinds.Count == 0)
                {
                    commonData.AllMilitaryKinds = CommonData.Current.AllMilitaryKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllSectionAIDetails.Count == 0)
                {
                    commonData.AllSectionAIDetails = CommonData.Current.AllSectionAIDetails;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllSkills.Count == 0)
                {
                    commonData.AllSkills = CommonData.Current.AllSkills;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllStratagems.Count == 0)
                {
                    commonData.AllStratagems = CommonData.Current.AllStratagems;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllStunts.Count == 0)
                {
                    commonData.AllStunts = CommonData.Current.AllStunts;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTechniques.Count == 0)
                {
                    commonData.AllTechniques = CommonData.Current.AllTechniques;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTerrainDetails.Count == 0)
                {
                    commonData.AllTerrainDetails = CommonData.Current.AllTerrainDetails;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTextMessages.Count == 0)
                {
                    commonData.AllTextMessages = CommonData.Current.AllTextMessages;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTileAnimations.Count == 0)
                {
                    commonData.AllTileAnimations = CommonData.Current.AllTileAnimations;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTitles.Count == 0)
                {
                    commonData.AllTitles = CommonData.Current.AllTitles;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTitleKinds.Count == 0)
                {
                    commonData.AllTitleKinds = CommonData.Current.AllTitleKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTroopAnimations.Count == 0)
                {
                    commonData.AllTroopAnimations = CommonData.Current.AllTroopAnimations;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTroopEventEffectKinds.Count == 0)
                {
                    commonData.AllTroopEventEffectKinds = CommonData.Current.AllTroopEventEffectKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTroopEventEffects.Count == 0)
                {
                    commonData.AllTroopEventEffects = CommonData.Current.AllTroopEventEffects;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllEventEffectKinds.Count == 0)
                {
                    commonData.AllEventEffectKinds = CommonData.Current.AllEventEffectKinds;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllEventEffects.Count == 0)
                {
                    commonData.AllEventEffects = CommonData.Current.AllEventEffects;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllBiographyAdjectives.Count == 0)
                {
                    commonData.AllBiographyAdjectives = CommonData.Current.AllBiographyAdjectives;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.PersonGeneratorSetting == null)
                {
                    commonData.PersonGeneratorSetting = CommonData.Current.PersonGeneratorSetting;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllPersonGeneratorTypes.Count == 0)
                {
                    commonData.AllPersonGeneratorTypes = CommonData.Current.AllPersonGeneratorTypes;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTrainPolicies.Count == 0)
                {
                    commonData.AllTrainPolicies = CommonData.Current.AllTrainPolicies;
                    scenario.UsingOwnCommonData = false;
                }
                if (commonData.AllTreasureCreationSettings.Count == 0)
                {
                    commonData.AllTreasureCreationSettings = CommonData.Current.AllTreasureCreationSettings;
                    scenario.UsingOwnCommonData = false;
                }
                #endregion
            }
            
            Session.Current.Scenario = scenario;

            scenario.ProcessScenarioData(fromScenario);

            return scenario;
        }

        public void LoadScenario(string filename, List<int> playerFactions, bool fromScenario, MainGameScreen mainGameScreen)
        {
            while (CommonData.CurrentReady == false)
            {
                Platform.Sleep(100);
            }

            string scenarioName;

            if (fromScenario)
            {
                scenarioName = $@"Content\Data\Scenario\{filename}.json";
            }
            else
            {
                const string saveDirectory = "Save";

                if (!PlatformBase.Current.UserDirectoryExist(saveDirectory))
                {
                    PlatformBase.Current.UserDirectoryCreate(saveDirectory);
                }

                scenarioName = $@"{saveDirectory}\{filename}.json";
            }

            LoadScenarioData(scenarioName, fromScenario);

            var scenario = Session.Current.Scenario;

            if (fromScenario)
            {
                scenario.PlayerList = playerFactions;
            }

            if (string.IsNullOrEmpty(scenario.CurrentPlayerID) && scenario.PlayerList.Count > 0)
            {
                scenario.CurrentPlayerID = scenario.PlayerList.FirstOrDefault().ToString();
            }

            if (scenario.PlayerList.Count > 0)
            {
                foreach (int i in scenario.PlayerList)
                {
                    scenario.PlayerFactions.Add(scenario.Factions.GetGameObject(i));
                }
                if (!string.IsNullOrEmpty(scenario.CurrentPlayerID))
                {
                    var currentPlayer = scenario.Factions.GetGameObject(int.Parse(scenario.CurrentPlayerID)) as Faction;
                    scenario.CurrentFaction = currentPlayer;
                    scenario.Factions.RunningFaction = currentPlayer;
                }
            }

            if (scenario.PlayerList.Count == 0)
            {
                Session.Current.Scenario.ForceOptionsOnAutoplay();
            }

            //this.Clear();
            //this.Factions.LoadQueueFromString(reader["FactionQueue"].ToString()); 
        }
        
        public void InitEvents()
        {
            Session.Current.Scenario.OnAfterLoadScenario += new GameScenario.AfterLoadScenario(Scenario_OnAfterLoadScenario);
            Session.Current.Scenario.OnNewFactionAppear += new GameScenario.NewFactionAppear(Scenario_OnNewFactionAppear);
            Session.Current.Scenario.Date.OnDayStarting += new GameDate.DayStartingEvent(this.Date_OnDayStarting);
            Session.Current.Scenario.Date.OnDayPassed += new GameDate.DayPassedEvent(this.Date_OnDayPassed);
            Session.Current.Scenario.Date.OnMonthPassed += new GameDate.MonthPassedEvent(this.Date_OnMonthPassed);
            Session.Current.Scenario.Date.OnSeasonChange += new GameDate.SeasonChangeEvent(this.Date_OnSeasonChange);
            Session.Current.Scenario.Date.OnYearStarting += new GameDate.YearStartingEvent(this.Date_OnYearStarting);
            Session.Current.Scenario.Date.OnYearPassed += new GameDate.YearPassedEvent(this.Date_OnYearPassed);
            //this.Player.PlayStateChange += (new _WMPOCXEvents_PlayStateChangeEventHandler(this.Player_PlayStateChange));
        }

        private void Scenario_OnAfterLoadScenario()
        {
            this.Textures.LoadTextures();

            base.DefaultMouseArrowTexture = this.Textures.MouseArrowTextures[0];

            if (Session.Current.Scenario.ScenarioMap != null)
            {
                this.mainMapLayer.PrepareMap();
                this.UpdateViewport();
                this.ResetScreenEdge();
                this.mainMapLayer.ReCalculateTileDestination(this);
                this.JumpTo(Session.Current.Scenario.ScenarioMap.JumpPosition);
            }
            if (this.Plugins.GameRecordPlugin.IsRecordShowing)
            {
                this.Plugins.GameRecordPlugin.AddDisableRects();
            }
            this.Plugins.AirViewPlugin.ResetMapPosition(this);
            this.Plugins.AirViewPlugin.ResetFramePosition(base.viewportSize, this.mainMapLayer.LeftEdge, this.mainMapLayer.TopEdge, this.mainMapLayer.TotalMapSize);
            if (Session.Current.Scenario.ScenarioMap.MapName != null)
            {
                this.Plugins.AirViewPlugin.ReloadAirView(Session.Current.Scenario.ScenarioMap.MapName + ".jpg");
            }
            else
            {
                this.Plugins.AirViewPlugin.ReloadAirView();
            }
            if (this.Plugins.AirViewPlugin.IsMapShowing)
            {
                this.Plugins.AirViewPlugin.AddDisableRects();
            }
        }
    }
}

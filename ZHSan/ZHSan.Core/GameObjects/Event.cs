using GameDatas;
using GameEvents;
using GameManager;
using GameObjects.ArchitectureDetail.EventEffect;
using GameObjects.Conditions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameObjects
{
    [DataContract]
    public class Event : GameObject
    {
        private EventManager eventManager = EventManager.Instance;

        /// <summary>
        /// 已发生过
        /// </summary>
        [DataMember]
        public bool happened { get; set; }

        /// <summary>
        /// 可以重复
        /// </summary>
        [DataMember]
        public bool repeatable { get; set; }

        /// <summary>
        /// 不重要 不重要的事件不会出现对话，除非涉及君主
        /// </summary>
        [DataMember]
        public bool Minor { get; set; }

        /// <summary>
        /// 某事件发生之后 需要在某事件发生过之后才能触发
        /// </summary>
        [DataMember]
        public int AfterEventHappened { get; set; } = -1;

        public TroopEvent AfterHappenedEvent;

        /// <summary>
        /// 发动几率 实际机率为1除以此数
        /// </summary>
        [DataMember]
        public int happenChance { get; set; }

        /// <summary>
        /// 全势力可见
        /// </summary>
        [DataMember]
        public bool GloballyDisplayed { get; set; }

        /// <summary>
        /// 开始年
        /// </summary>
        [DataMember]
        public int StartYear { get; set; }= 0;

        /// <summary>
        /// 开始月
        /// </summary>
        [DataMember]
        public int StartMonth { get; set; } = 1;

        /// <summary>
        /// 结束年
        /// </summary>
        [DataMember]
        public int EndYear { get; set; } = 99999;

        /// <summary>
        /// 结束月
        /// </summary>
        [DataMember]
        public int EndMonth { get; set; } = 12;

        /// <summary>
        /// 武将编号 指定可能触发的武将ID，以空格分隔，先指定第k个武将，后跟随一个武将ID，如0 100 0 234 1 346 2 -1 可在同一个k指定多个武将，则代表列表中任何一个 -1代表任何武将
        /// </summary>
        [DataMember]
        public string personString { get; set; }

        /// <summary>
        /// 武将条件 触发的武将需符合的条件，以空格分隔，先指定第k个武将，后跟随一个武将ID
        /// </summary>
        [DataMember]
        public string PersonCondString { get; set; }

        /// <summary>
        /// 建筑编号 触发时，指定所有武将所在建筑的ID，以空格分隔 留空代表任何建筑
        /// </summary>
        [DataMember]
        public string architectureString { get; set; }

        /// <summary>
        /// 建筑条件 触发时，所有武将所在的建筑需符合的条件 如使用武将条件，将检查该建筑的县令
        /// </summary>
        [DataMember]
        public string architectureCondString { get; set; }

        /// <summary>
        /// 势力编号 触发时，指定所有武将所在势力的ID，以空格分隔 留空代表任何势力
        /// </summary>
        [DataMember]
        public string factionString { get; set; }

        /// <summary>
        /// 势力条件 触发时，所有武将所在的势力需符合的条件 如使用武将条件，将检查该势力的君主
        /// </summary>
        [DataMember]
        public string factionCondString { get; set; }

        /// <summary>
        /// 对话 先指定第k个武将，后跟随一段对话 以空格分隔 可使用%k表示第k个武将的姓名
        /// </summary>
        [DataMember]
        public string dialogString { get; set; }

        /// <summary>
        /// 效果 先指定第k个武将 后跟随一个效果种类 以空格分隔
        /// </summary>
        [DataMember]
        public string effectString { get; set; }

        /// <summary>
        /// 建筑效果 武将所在建筑效果，以空格分隔 如使用武将效果，将应用于该建筑的县令
        /// </summary>
        [DataMember]
        public string architectureEffectString { get; set; }

        /// <summary>
        /// 势力效果 武将所在势力效果，以空格分隔 如使用武将效果，将应用于该势力的君主
        /// </summary>
        [DataMember]
        public string factionEffectIDString { get; set; }

        /// <summary>
        /// 图片 图片档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录tupian里
        /// </summary>
        [DataMember]
        public string Image { get; set; }

        /// <summary>
        /// 音效 音效档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录yinxiao里
        /// </summary>
        [DataMember]
        public string Sound { get; set; }

        /// <summary>
        /// 选是的对话 先指定第k个武将，后跟随一段对话 以空格分隔 可使用%k表示第k个武将的姓名
        /// </summary>
        [DataMember]
        public string yesdialogString { get; set; }

        /// <summary>
        /// 选否的对话 先指定第k个武将，后跟随一段对话 以空格分隔 可使用%k表示第k个武将的姓名
        /// </summary>
        [DataMember]
        public string nodialogString { get; set; }

        /// <summary>
        /// 选是的效果 如果填上，这事件会有选项 选是后执行这些效果 先指定第k个武将，后跟随一个效果种类 以空格分隔
        /// </summary>
        [DataMember]
        public string yesEffectString { get; set; }

        /// <summary>
        /// 选否的效果 如果填上，这事件会有选项 选否后执行这些效果 先指定第k个武将，后跟随一个效果种类 以空格分隔
        /// </summary>
        [DataMember]
        public string noEffectString { get; set; }

        /// <summary>
        /// 选是的建筑效果 武将所在建筑效果，以空格分隔 如使用武将效果，将应用于该建筑的县令
        /// </summary>
        [DataMember]
        public string yesArchitectureEffectString { get; set; }

        /// <summary>
        /// 选否的建筑效果 武将所在建筑效果，以空格分隔 如使用武将效果，将应用于该建筑的县令
        /// </summary>
        [DataMember]
        public string noArchitectureEffectString { get; set; }

        /// <summary>
        /// 武将列传 先指定第k个武将，后跟随一段武将列传 以空格分隔 可使用%k表示第k个武将的姓名
        /// </summary>
        [DataMember]
        public string scenBiographyString { get; set; }

        /// <summary>
        /// 下一剧本，暂时无用
        /// </summary>
        [DataMember]
        public string nextScenario { get; set; }

        [DataMember]
        public string TryToShowString { get; set; }

        public Dictionary<int, List<Person>> person;

        public Dictionary<int, List<Condition>> personCond;

        public List<Architecture> Architectures { get; set; }

        public List<Condition> architectureCond;

        public List<Faction> Factions { get; set; } = new();

        public List<Condition> factionCond;
        
        public List<PersonIdDialog> dialog;

        public Dictionary<int, List<EventEffect>> effect;
        public List<PersonDialog> matchedDialog;
        public Dictionary<Person, List<EventEffect>> matchedEffect;

        public List<PersonDialog> matchedyesDialog = new List<PersonDialog>();
        public List<PersonDialog> matchednoDialog = new List<PersonDialog>();
        
        private List<PersonIdDialog> yesDialogs = new();
        public List<PersonIdDialog> nodialog = new();

        public Dictionary<Person, List<EventEffect>> matchedYesEffect;
        public Dictionary<Person, List<EventEffect>> matchedNoEffect;

        public Dictionary<int, List<EventEffect>> yesEffect = new Dictionary<int,List<EventEffect>>();
        public Dictionary<int, List<EventEffect>> noEffect = new Dictionary<int,List<EventEffect>>();

        public List<EventEffect> architectureEffect = new List<EventEffect>();

        public List<EventEffect> factionEffect = new List<EventEffect>();

        public List<EventEffect> yesArchitectureEffect = new List<EventEffect>();
        public List<EventEffect> noArchitectureEffect = new List<EventEffect>();

        public List<PersonIdDialog> scenBiography = new List<PersonIdDialog>();
        
        public List<PersonDialog> matchedScenBiography = new List<PersonDialog> ();

        private bool involveLeader = false;
        public bool InvolveLeader
        {
            get
            {
                return involveLeader;
            }
        }

        public Event(EventConfig config)
        {
            ID = config.Id;
            Name = config.Name;
            happened = config.Happened;
            repeatable = config.Repeatable;
            Minor = config.Minor;
            AfterEventHappened = config.AfterEventHappened;
            happenChance = config.HappenChance;
            GloballyDisplayed = config.GloballyDisplayed;
            StartYear = config.StartYear;
            StartMonth = config.StartMonth;
            EndYear = config.EndYear;
            EndMonth = config.EndMonth;
            personString = config.PersonString;
            PersonCondString = config.PersonCondString;
            architectureString = config.ArchitectureString;
            architectureCondString = config.ArchitectureCondString;
            factionString = config.FactionString;
            dialogString = config.DialogString;
            effectString = config.EffectString;
            architectureEffectString = config.ArchitectureEffectString;
            factionEffectIDString = config.FactionEffectIDString;
            Image = config.Image;
            Sound = config.Sound;
            yesdialogString = config.YesDialogString;
            nodialogString = config.NoDialogString;
            yesEffectString = config.YesEffectString;
            noEffectString = config.NoEffectString;
            yesArchitectureEffectString = config.YesArchitectureEffectString;
            noArchitectureEffectString = config.NoArchitectureEffectString;
            scenBiographyString = config.ScenBiographyString;
            nextScenario = config.NextScenario;
            TryToShowString = config.TryToShowString;
        }
        
        public EventConfig ToConfig()
        {
            return new EventConfig
            {
                Id = ID,
                Name = Name,
                Happened = happened,
                Repeatable = repeatable,
                Minor = Minor,
                AfterEventHappened = AfterEventHappened,
                HappenChance = happenChance,
                GloballyDisplayed = GloballyDisplayed,
                StartYear = StartYear,
                StartMonth = StartMonth,
                EndYear = EndYear,
                EndMonth = EndMonth,
                PersonString = personString,
                PersonCondString = PersonCondString,
                ArchitectureString = architectureString,
                ArchitectureCondString = architectureCondString,
                FactionString = factionString,
                FactionCondString = factionCondString,
                DialogString = dialogString,
                EffectString = effectString,
                ArchitectureEffectString = architectureEffectString,
                FactionEffectIDString = factionEffectIDString,
                Image = Image,
                Sound = Sound,
                YesDialogString = yesdialogString,
                NoDialogString = nodialogString,
                YesEffectString = yesEffectString,
                NoEffectString = noEffectString,
                YesArchitectureEffectString = yesArchitectureEffectString,
                NoArchitectureEffectString = noArchitectureEffectString,
                ScenBiographyString = scenBiographyString,
                NextScenario = nextScenario,
                TryToShowString = TryToShowString,
            };
        }

        public void Init()
        {
            yesEffect = new Dictionary<int, List<EventEffect>>();

            noEffect = new Dictionary<int, List<EventEffect>>();

            architectureEffect = new List<EventEffect>();

            factionEffect = new List<EventEffect>();

            yesArchitectureEffect = new List<EventEffect>();

            noArchitectureEffect = new List<EventEffect>();

            if (dialog == null)
            {
                dialog = new List<PersonIdDialog>();
            }
            yesDialogs = new();
            
            if (nodialog == null)
            {
                nodialog = new List<PersonIdDialog>();
            }
            if (scenBiography == null)
            {
                scenBiography = new List<PersonIdDialog>();
            }
        }

        public void ApplyEventDialogs(Architecture a, Screen screen)
        {
            Session.Current.Scenario = Session.Current.Scenario;
            eventManager.Publish(new ArchitectureEvent(this, a, screen));
            
            foreach (PersonDialog i in matchedScenBiography) 
            {
                if (i.SpeakingPerson != null)
                {
                    Session.Current.Scenario.YearTable.addPersonInGameBiography(i.SpeakingPerson, Session.Current.Scenario.Date, i.Text);
                }
            }
            if (nextScenario != null && nextScenario.Length > 0)
            {
                // Session.Current.Scenario.EnableLoadAndSave = false;

                List<int> factionIds = new List<int>();
                foreach (Faction f in Session.Current.Scenario.PlayerFactions) 
                {
                    factionIds.Add(f.ID);
                }

                //暫時取消
                //OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
                //{
                //    DataSource = "GameData/Scenario/" + nextScenario,
                //    Provider = "Microsoft.Jet.OLEDB.4.0"
                //};
                //Session.Current.Scenario.LoadGameScenarioFromDatabase(builder.ConnectionString, factionIds);

                //Session.MainGame.mainGameScreen.ReloadScreenData();

                //Session.Current.Scenario.EnableLoadAndSave = true;
            }
        }

        public void DoYesApplyEvent(Architecture a)
        {
            if (yesEffect != null)
            {
                foreach (KeyValuePair<Person, List<EventEffect>> i in matchedYesEffect)
                {
                    foreach (EventEffect j in i.Value)
                    {
                        j.ApplyEffect(i.Key, this);
                    }
                }

                foreach (PersonDialog dialog in matchedyesDialog)
                {
                    var person = dialog.SpeakingPerson ?? a.BelongedFaction.Leader;

                    Session.MainGame.mainGameScreen.xianshishijiantupian(person, null, dialog.Text, true);
                }
            }

            if (yesArchitectureEffect != null)
            {
                foreach (EventEffect i in yesArchitectureEffect)
                {
                    i.ApplyEffect(a, this);
                }
            }
        }

        public void DoNoApplyEvent(Architecture a)
        {
            if (this.noEffect != null)
            {
                foreach (KeyValuePair<Person, List<EventEffect>> i in matchedNoEffect)
                {
                    foreach (EventEffect j in i.Value)
                    {
                        j.ApplyEffect(i.Key, this);
                    }
                }
                foreach (PersonDialog nodialog in this.matchednoDialog)
                {
                    if (nodialog.SpeakingPerson != null)
                    {
                        Session.MainGame.mainGameScreen.xianshishijiantupian(nodialog.SpeakingPerson, null, nodialog.Text, true);
                    }
                    else
                    {
                        Session.MainGame.mainGameScreen.xianshishijiantupian(a.BelongedFaction.Leader, null, nodialog.Text, true);
                    }
                }
            }
            if (this.noArchitectureEffect != null)
            {
                foreach (EventEffect j in noArchitectureEffect)
                {
                    j.ApplyEffect(a, this);
                }

            }
        }

        public void DoApplyEvent(Architecture a)
        {
            if (matchedEffect != null)
            {
                foreach (KeyValuePair<Person, List<EventEffect>> i in matchedEffect)
                {
                    foreach (EventEffect j in i.Value)
                    {
                        j.ApplyEffect(i.Key, this);
                    }
                }
            }
            if (architectureEffect != null)
            {
                foreach (EventEffect i in architectureEffect)
                {
                    i.ApplyEffect(a, this);
                }
            }
            if (factionEffect != null && a.BelongedFaction != null)
            {
                foreach (EventEffect i in factionEffect)
                {
                    i.ApplyEffect(a.BelongedFaction, this);
                }
            }
        }

        public bool matchEventPersons(Architecture a)
        {
            GameObjectList allPersons = a.AllPersonAndChildren.GetList();

            HashSet<int> haveCond = new HashSet<int>();
            foreach (KeyValuePair<int, List<Condition>> i in this.personCond)
            {
                haveCond.Add(i.Key);
            }

            HashSet<int> noCond = new HashSet<int>();
            foreach (KeyValuePair<int, List<Person>> i in this.person)
            {
                if (!haveCond.Contains(i.Key) && i.Value.Count == 0)
                {
                    noCond.Add(i.Key);
                }
            }

            Dictionary<int, List<Person>> candidates = new Dictionary<int, List<Person>>();
            foreach (int i in this.person.Keys)
            {
                candidates[i] = new List<Person>();
                if (noCond.Contains(i))
                {
                    foreach (Person p in allPersons.GetList())
                    {
                        candidates[i].Add(p);
                    }
                }
            }

            foreach (KeyValuePair<int, List<Condition>> i in this.personCond)
            {
                foreach (Person p in allPersons)
                {
                    bool ok = Condition.CheckConditionList(i.Value, p);
                    if (ok)
                    {
                        if (this.person[i.Key].Contains(null) || this.person[i.Key].Contains(p))
                        {
                            candidates[i.Key].Add(p);
                        }
                    }
                }
            }
            // check 7000 - 8000 persons which can be in anywhere
            foreach (KeyValuePair<int, List<Person>> i in this.person)
            {
                foreach (Person p in i.Value)
                {
                    if (p != null /*&& p.ID >= 7000 && p.ID < 8000*/)
                    {
                        bool ok;
                        if (this.personCond.ContainsKey(i.Key))
                        {
                            ok = Condition.CheckConditionList(this.personCond[i.Key], p);
                        }
                        else
                        {
                            ok = true;
                        }
                        if (ok)
                        {
                            if (this.person[i.Key].Contains(null) || this.person[i.Key].Contains(p))
                            {
                                candidates[i.Key].Add(p);
                            }
                        }
                    }
                }
            }

            foreach (List<Person> i in candidates.Values)
            {
                if (i.Count == 0) return false;
            }

            Dictionary<int, Person> matchedPersons = new Dictionary<int, Person>();
            foreach (KeyValuePair<int, List<Person>> i in candidates)
            {
                if (i.Value.Count <= 0) return false;
                Person selected = i.Value[GameObject.Random(i.Value.Count)];
                matchedPersons[i.Key] = selected;
                foreach (List<Person> j in candidates.Values)
                {
                    j.Remove(selected);
                }
            }

            matchedDialog = new List<PersonDialog>();
            foreach (PersonIdDialog i in this.dialog)
            {
                if (!matchedPersons.ContainsKey(i.id)) return false;

                PersonDialog pd = new PersonDialog();
                pd.SpeakingPerson = matchedPersons[i.id];
                pd.Text = i.dialog;
                for (int j = 0; j < matchedPersons.Count; ++j)
                {
                    pd.Text = pd.Text.Replace("%" + j, matchedPersons[j].Name);
                }
                matchedDialog.Add(pd);
            }

            matchedyesDialog = new List<PersonDialog>();

            foreach (var dialog in yesDialogs)
            {
                if (!matchedPersons.ContainsKey(dialog.id)) return false;

                string text = dialog.yesdialog;
                for (int j = 0; j < matchedPersons.Count; ++j)
                {
                    text = text.Replace("%" + j, ' ' + matchedPersons[j].Name + ' ');
                }
                
                matchedyesDialog.Add(new PersonDialog
                {
                    SpeakingPerson = matchedPersons[dialog.id],
                    Text = text,
                });
            }

            matchednoDialog = new List<PersonDialog>();
            foreach (PersonIdDialog i in this.nodialog)
            {
                if (!matchedPersons.ContainsKey(i.id)) return false;

                PersonDialog pd = new PersonDialog();
                pd.SpeakingPerson = matchedPersons[i.id];
                pd.Text = i.nodialog;
                for (int j = 0; j < matchedPersons.Count; ++j)
                {
                    pd.Text = pd.Text.Replace("%" + j, ' ' + matchedPersons[j].Name + ' ');
                }
                matchednoDialog.Add(pd);
            }

            matchedScenBiography = new List<PersonDialog>();
            foreach (PersonIdDialog i in this.scenBiography)
            {
                if (!matchedPersons.ContainsKey(i.id)) return false;

                PersonDialog pd = new PersonDialog();
                pd.SpeakingPerson = matchedPersons[i.id];
                pd.Text = i.dialog;
                for (int j = 0; j < matchedPersons.Count; ++j)
                {
                    pd.Text = pd.Text.Replace("%" + j, matchedPersons[j].Name);
                }
                matchedScenBiography.Add(pd);
            }

            matchedEffect = new Dictionary<Person, List<EventEffect>>();
            foreach (KeyValuePair<int, List<EventEffect>> i in this.effect)
            {
                matchedEffect.Add(matchedPersons[i.Key], i.Value);
            }
            matchedYesEffect = new Dictionary<Person, List<EventEffect>>();
            foreach (KeyValuePair<int, List<EventEffect>> i in this.yesEffect)
            {
                matchedYesEffect.Add(matchedPersons[i.Key], i.Value);
            }
            matchedNoEffect = new Dictionary<Person, List<EventEffect>>();
            foreach (KeyValuePair<int, List<EventEffect>> i in this.noEffect)
            {
                matchedNoEffect.Add(matchedPersons[i.Key], i.Value);
            }

            if (a.BelongedFaction != null)
            {
                foreach (Person p in matchedPersons.Values)
                {
                    if (p == a.BelongedFaction.Leader && Session.Current.Scenario.IsPlayer(a.BelongedFaction))
                    {
                        involveLeader = true;
                    }
                }
            }

            return true;
        }

        public bool checkConditions(Architecture a)
        {
            if (this.happened && !this.repeatable) return false;
            if (GameObject.Random(this.happenChance) != 0)
            {
                return false;
            }

            var afterEvent = Session.Current.Scenario.AllEvents.GetValueOrDefault(AfterEventHappened);
            if (afterEvent == null || !afterEvent.happened) return false;

            if (Session.Current.Scenario.Date.Year < this.StartYear || Session.Current.Scenario.Date.Year > this.EndYear) return false;

            if (Session.Current.Scenario.Date.Year == this.StartYear)
            {
                if (Session.Current.Scenario.Date.Month < this.StartMonth) return false;
            }

            if (Session.Current.Scenario.Date.Year == this.EndYear)
            {
                if (Session.Current.Scenario.Date.Month > this.EndMonth) return false;
            }

            if (!Condition.CheckConditionList(this.architectureCond, a)) return false;
            if (!Condition.CheckConditionList(this.factionCond, a.BelongedFaction)) return false;

            if (Factions.Count > 0)
            {
                bool contains = false;
                foreach (var architecture in Architectures)
                {
                    if (architecture.ID == a.ID)
                    {
                        contains = true;
                    }
                }

                if (Factions != null)
                {
                    foreach (var faction in Factions)
                    {
                        if (a.BelongedFaction != null && faction != null && faction.ID == a.BelongedFaction.ID)
                        {
                            contains = true;
                        }
                    }

                }

                return contains;
            }
            
            return this.matchEventPersons(a);
        }

        public bool IsStart()
        {
            if (Session.Current.Scenario.GameCommonData.AllConditions.TryGetValue(9998, out var condition))
            {
                return architectureCond.Contains(condition) || factionCond.Contains(condition);
            }

            return false;
        }

        public bool IsEnd()
        {
            if (Session.Current.Scenario.GameCommonData.AllConditions.TryGetValue(9999, out var condition))
            {
                return architectureCond.Contains(condition) || factionCond.Contains(condition);
            }

            return false;
        }

        public Dictionary<int, List<Person>> LoadPersonIdFromString(Dictionary<int, Person> persons, string data)
        {
            var result = new Dictionary<int, List<Person>>();
            string[] strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strArray.Length; i += 2)
            {
                int key = int.Parse(strArray[i]);
                int personId = int.Parse(strArray[i + 1]);

                if (!result.ContainsKey(key))
                {
                    result.Add(key, new List<Person>());
                }

                if (persons.ContainsKey(personId))
                {
                    result[key].Add(persons[personId]);
                }
            }

            return result;
        }

        public void LoadyesDialogFromString(string data)
        {
            var result = new List<PersonIdDialog>();

            var strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strArray.Length; i += 2)
            {
                result.Add(new PersonIdDialog
                {
                    id = int.Parse(strArray[i]),
                    yesdialog = strArray[i + 1],
                });
            }

            yesDialogs = result;
        }

        public List<PersonIdDialog> LoadDialogsFromString(string data)
        {
            var result = new List<PersonIdDialog>();

            var strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strArray.Length; i += 2)
            {
                result.Add(new PersonIdDialog
                {
                    id = int.Parse(strArray[i]),
                    dialog = strArray[i + 1],
                });
            }

            return result;
        }

        public string SaveDialogToString()
        {
            var result = SaveDialogToString(dialog);

            return result;
        }

        public string SaveyesDialogToString()
        {
            var sb = new StringBuilder();

            foreach (var personIdDialog in yesDialogs)
            {
                sb.Append(personIdDialog.id).Append(' ').Append(personIdDialog.yesdialog).Append(' ');
            }

            return sb.ToString();
        }

        public string SavenoDialogToString()
        {
            var result = SaveDialogToString(nodialog);

            return result;
        }
        
        public string SaveScenBiographyToString()
        {
            var result = SaveDialogToString(scenBiography);

            return result;
        }

        private string SaveDialogToString(List<PersonIdDialog> dialogs)
        {
            var sb = new StringBuilder();

            foreach (var personIdDialog in dialogs)
            {
                sb.Append(personIdDialog.id).Append(' ').Append(personIdDialog.dialog).Append(' ');
            }

            return sb.ToString();
        }
        
       /*
        public bool CheckFactionEvent(Architecture a)
        {
           if (this.faction != null && this.faction.GameObjects.Contains(a.BelongedFaction) && checkConditions(a))
            {
                return true ;
            }
            return false ;
        }
        */
    }
}
using GameManager;
using GameObjects.ArchitectureDetail.EventEffect;
using GameObjects.Conditions;
using GameObjects.TroopDetail;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class PersonIdDialog
    {
        [DataMember]
        public int id;
        [DataMember]
        public string dialog;
        [DataMember]
        public string yesdialog;
        [DataMember]
        public string nodialog;
    }

    [DataContract]
    public class Event : GameObject
    {
        [DataMember]
        public int AfterEventHappened = -1;
        public TroopEvent AfterHappenedEvent;
        [DataMember]
        public int happenChance;

        [DataMember]
        public bool happened;
        [DataMember]
        public bool repeatable;

        [DataMember]
        public String nextScenario;

        [DataMember]
        public string personString { get; set; }

        public Dictionary<int, List<Person>> person;

        [DataMember]
        public string PersonCondString { get; set; }

        public Dictionary<int, List<Condition>> personCond;

        [DataMember]
        public string architectureString { get; set; }

        public List<Architecture> Architectures { get; set; }

        [DataMember]
        public string architectureCondString { get; set; }

        public List<Condition> architectureCond;

        [DataMember]
        public string factionString { get; set; }

        public FactionList faction;

        [DataMember]
        public string factionCondString { get; set; }

        public List<Condition> factionCond;
        
        public List<PersonIdDialog> dialog;

        [DataMember]
        public string dialogString { get; set; }

        [DataMember]
        public string effectString { get; set; }

        public Dictionary<int, List<EventEffect>> effect;
        public List<PersonDialog> matchedDialog;
        public Dictionary<Person, List<EventEffect>> matchedEffect;

        public List<PersonDialog> matchedyesDialog = new List<PersonDialog>();
        public List<PersonDialog> matchednoDialog = new List<PersonDialog>();
        
        public List<PersonIdDialog> yesdialog = new List<PersonIdDialog>();
        public List<PersonIdDialog> nodialog = new List<PersonIdDialog>();

        [DataMember]
        public string yesdialogString { get; set; }
        [DataMember]
        public string nodialogString { get; set; }

        public Dictionary<Person, List<EventEffect>> matchedYesEffect;
        public Dictionary<Person, List<EventEffect>> matchedNoEffect;

        [DataMember]
        public string yesEffectString { get; set; }

        [DataMember]
        public string noEffectString { get; set; }

        public Dictionary<int, List<EventEffect>> yesEffect = new Dictionary<int,List<EventEffect>>();
        public Dictionary<int, List<EventEffect>> noEffect = new Dictionary<int,List<EventEffect>>();

        [DataMember]
        public string architectureEffectString { get; set; }

        public List<EventEffect> architectureEffect = new List<EventEffect>();

        [DataMember]
        public string factionEffectIDString { get; set; }

        public List<EventEffect> factionEffect = new List<EventEffect>();

        [DataMember]
        public string yesArchitectureEffectString { get; set; }

        [DataMember]
        public string noArchitectureEffectString { get; set; }

        public List<EventEffect> yesArchitectureEffect = new List<EventEffect>();
        public List<EventEffect> noArchitectureEffect = new List<EventEffect>();

        public List<PersonIdDialog> scenBiography = new List<PersonIdDialog>() ;

        [DataMember]
        public string scenBiographyString { get; set; }
        
        public List<PersonDialog> matchedScenBiography = new List<PersonDialog> () ;

        [DataMember]
        public String Image = "";
        [DataMember]
        public String Sound = "";
        [DataMember]
        public bool GloballyDisplayed = false;
        [DataMember]
        public int StartYear = 0;
        [DataMember]
        public int StartMonth = 1;
        [DataMember]
        public int EndYear = 99999;
        [DataMember]
        public int EndMonth = 12;

        [DataMember]
        public bool Minor = false;

        [DataMember]
        public string TryToShowString { get; set; }

        private bool involveLeader = false;
        public bool InvolveLeader
        {
            get
            {
                return involveLeader;
            }
        }

        public event ApplyEvent OnApplyEvent;

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
            if (yesdialog == null)
            {
                yesdialog = new List<PersonIdDialog>();
            }
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
            if (this.OnApplyEvent != null)
            {
                this.OnApplyEvent(this, a, screen);
            }
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
            if (this.yesEffect != null)
            {

                foreach (KeyValuePair<Person, List<EventEffect>> i in matchedYesEffect)
                {
                    foreach (EventEffect j in i.Value)
                    {
                        j.ApplyEffect(i.Key, this);
                    }
                }
                foreach (PersonDialog yesdialog in this.matchedyesDialog)
                {
                    if (yesdialog.SpeakingPerson != null)
                    {
                        Session.MainGame.mainGameScreen.xianshishijiantupian(yesdialog.SpeakingPerson, null, yesdialog.Text, true);
                    }
                    else
                    {
                        Session.MainGame.mainGameScreen.xianshishijiantupian(a.BelongedFaction.Leader, null, yesdialog.Text, true);
                    }
                }
            }
            if (this.yesArchitectureEffect != null)
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
            foreach (PersonIdDialog i in this.yesdialog)
            {
                if (!matchedPersons.ContainsKey(i.id)) return false;

                PersonDialog pd = new PersonDialog();
                pd.SpeakingPerson = matchedPersons[i.id];
                pd.Text = i.yesdialog;
                for (int j = 0; j < matchedPersons.Count; ++j)
                {
                    pd.Text = pd.Text.Replace("%" + j, ' ' + matchedPersons[j].Name + ' ');
                }
                matchedyesDialog.Add(pd);
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

            if (this.AfterEventHappened >= 0)
            {
                if (!(Session.Current.Scenario.AllEvents.GetGameObject(this.AfterEventHappened) as Event).happened)
                {
                    return false;
                }
            }

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

            if (faction.Count > 0)
            {
                bool contains = false;
                foreach (var architecture in Architectures)
                {
                    if (architecture.ID == a.ID)
                    {
                        contains = true;
                    }
                }

                if (faction != null)
                {
                    foreach (Faction f in faction)
                    {
                        if (a.BelongedFaction != null && f != null)
                        {
                            if (f.ID == a.BelongedFaction.ID)
                            {
                                contains = true;
                            }
                        }
                    }

                }
                if (contains)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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

        public void LoadFactionFromString(FactionList factions, string data)
        {
            char[] separator = new char[] { ' ', '\n', '\r', '\t' };
            string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            this.faction = new FactionList();
            foreach (string i in strArray)
            {
                this.faction.Add(factions.GetGameObject(int.Parse(i)) as Faction);
            }
        }

        public void LoadDialogFromString(string data)
        {
            char[] separator = new char[] { ' ', '\n', '\r', '\t' };
            string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            this.dialog = new List<PersonIdDialog>();
            for (int i = 0; i < strArray.Length; i += 2)
            {
                PersonIdDialog d = new PersonIdDialog();
                d.id = int.Parse(strArray[i]);
                d.dialog = strArray[i + 1];
                this.dialog.Add(d);
            }
        }

        public void LoadyesDialogFromString(string data)
        {
            char[] separator = new char[] { ' ', '\n', '\r', '\t' };
            string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            this.yesdialog = new List<PersonIdDialog>();
            for (int i = 0; i < strArray.Length; i += 2)
            {
                PersonIdDialog d = new PersonIdDialog();
                d.id = int.Parse(strArray[i]);
                d.yesdialog = strArray[i + 1];
                this.yesdialog.Add(d);
            }
        }

        public void LoadnoDialogFromString(string data)
        {
            char[] separator = new char[] { ' ', '\n', '\r', '\t' };
            string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            this.nodialog = new List<PersonIdDialog>();
            for (int i = 0; i < strArray.Length; i += 2)
            {
                PersonIdDialog d = new PersonIdDialog();
                d.id = int.Parse(strArray[i]);
                d.nodialog = strArray[i + 1];
                this.nodialog.Add(d);
            }
        }
        
        public void LoadScenBiographyFromString(string data)
        {
            char[] separator = new char[] { ' ', '\n', '\r', '\t' };
            string[] strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            this.scenBiography = new List<PersonIdDialog>();
            for (int i = 0; i < strArray.Length; i += 2)
            {
                PersonIdDialog d = new PersonIdDialog();
                d.id = int.Parse(strArray[i]);
                d.dialog = strArray[i + 1];
                this.scenBiography.Add(d);
            }
        }

        public string SaveDialogToString()
        {
            string result = "";
            foreach (PersonIdDialog i in this.dialog)
            {
                result += i.id + " " + i.dialog + " ";
            }
            return result;
        }

        public string SaveyesDialogToString()
        {
            string result = "";
            foreach (PersonIdDialog i in this.yesdialog)
            {
                result += i.id + " " + i.yesdialog + " ";
            }
            return result;
        }

        public string SavenoDialogToString()
        {
            string result = "";
            foreach (PersonIdDialog i in this.nodialog)
            {
                result += i.id + " " + i.nodialog + " ";
            }
            return result;
        }
        
        public string SaveScenBiographyToString()
        {
            string result = "";
            foreach (PersonIdDialog i in this.scenBiography)
            {
                result += i.id + " " + i.dialog + " ";
            }
            return result;
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
        public delegate void ApplyEvent(Event te, Architecture a, Screen screen);
    }
}
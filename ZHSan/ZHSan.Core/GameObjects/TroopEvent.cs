using GameDatas;
using GameEnums;
using GameEvents;
using GameManager;
using GameObjects.Conditions;
using GameObjects.TroopDetail.EventEffect;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameObjects
{
    [DataContract]
    public class TroopEvent : GameObject
    {
        private EventManager eventManager = EventManager.Instance;

        /// <summary>
        /// 已发生过
        /// </summary>
        [DataMember]
        public bool Happened { get; set; }

        /// <summary>
        /// 可以重复
        /// </summary>
        [DataMember]
        public bool Repeatable { get; set; }

        /// <summary>
        /// 某事件发生之后：需要在某事件发生过之后才能触发
        /// </summary>
        [DataMember]
        public int AfterEventHappened { get; set; } = -1;

        /// <summary>
        /// 发动人物：发动事件的人物，所有效果将以本人物所在部队为中心。如果为-1，则每个部队都可以触发此事件。
        /// </summary>
        [DataMember]
        public int LaunchPersonString { get; set; }

        /// <summary>
        /// 人物对话：每个人物ID后跟随其要说的话。以空格分隔。留空则无对话。
        /// </summary>
        [DataMember]
        public string dialogString { get; set; }

        /// <summary>
        /// 发动条件：发动事件的部队所需要满足的条件，留空则为满足。
        /// </summary>
        [DataMember]
        public string ConditionsString { get; set; }

        /// <summary>
        /// 发动几率：0--100
        /// </summary>
        [DataMember]
        public int HappenChance { get; set; }

        /// <summary>
        /// 目标人物列表：每个关系之后跟随一个人物ID。0：非友好；1：友好。留空则不在搜索范围检查此条件。
        /// </summary>
        [DataMember]
        public string TargetPersonsString { get; set; }

        /// <summary>
        /// 自身效果：发动部队的效果列表
        /// </summary>
        [DataMember]
        public string SelfEffectsString { get; set; }

        /// <summary>
        /// 特定人物效果：每个人物ID后跟随一个效果种类。以空格分隔。
        /// </summary>
        [DataMember]
        public string EffectPersonsString { get; set; }

        /// <summary>
        /// 特定范围效果：范围类别：
        /// 0：视野内所有敌军；
        /// 1：视野内所有友军；
        /// 2：攻击范围内所有敌军；
        /// 3：攻击范围内所有友军；
        /// 4：周围八格内所有敌军；
        /// 5：周围八格内所有友军；
        /// 每个范围类别后跟随一个效果种类。以空格分隔。
        /// </summary>
        [DataMember]
        public string EffectAreasString { get; set; }

        /// <summary>
        /// 图片。图片档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录tupian里
        /// </summary>
        [DataMember]
        public string Image { get; set; }

        /// <summary>
        /// 音效。音效档案放在Content目录Textures目录GameComponents目录tupianwenzi目录Data目录yinxiao里
        /// </summary>
        [DataMember]
        public string Sound { get; set; }

        /// <summary>
        /// 0--视野内 1--周边八格 2--攻击范围 用来搜索目标人物列表
        /// </summary>
        [DataMember]
        public EventCheckAreaKind CheckArea { get; set; }

        [DataMember]
        public string TryToShowString { get; set; }

        public TroopEvent AfterHappenedEvent;

        public List<Condition> Conditions { get; set; } = new();

        //[DataMember]
        public List<PersonDialog> Dialogs = new();
        
        public Person LaunchPerson;

        private List<TroopEffectArea> effectAreas = new();

        private List<TroopEffectPerson> effectPersons = new();

        private List<PersonRelation> TargetPersons = new();

        public List<EventEffect> SelfEffects { get; set; } = new();

        public TroopEvent(TroopEventConfig config)
        {
            ID = config.Id;
            Name = config.Name;
            Happened = config.Happened;
            Repeatable = config.Repeatable;
            AfterEventHappened = config.AfterEventHappened;
            LaunchPersonString = config.LaunchPersonString;
            dialogString = config.DialogString;
            ConditionsString = config.ConditionsString;
            HappenChance = config.HappenChance;
            TargetPersonsString = config.TargetPersonsString;
            SelfEffectsString = config.SelfEffectsString;
            EffectPersonsString = config.EffectPersonsString;
            EffectAreasString = config.EffectAreasString;
            Image = config.Image;
            Sound = config.Sound;
            CheckArea = config.CheckArea;
            TryToShowString = config.TryToShowString;
        }

        public TroopEventConfig ToConfig()
        {
            return new TroopEventConfig
            {
                Id = ID,
                Name = Name,
                Happened = Happened,
                Repeatable = Repeatable,
                AfterEventHappened = AfterEventHappened,
                LaunchPersonString = LaunchPersonString,
                DialogString = dialogString,
                ConditionsString = ConditionsString,
                HappenChance = HappenChance,
                TargetPersonsString = TargetPersonsString,
                SelfEffectsString = SelfEffectsString,
                EffectPersonsString = EffectPersonsString,
                EffectAreasString = EffectAreasString,
                Image = Image,
                Sound = Sound,
                CheckArea = CheckArea,
                TryToShowString = TryToShowString,
            };
        }

        public void Init()
        {
            eventManager = EventManager.Instance;

            if (Dialogs == null)
            {
                Dialogs = new List<PersonDialog>();
            }
        }

        public void ApplyEventDialogs(Troop troop)
        {
            eventManager.Publish(new ApplyTroopEvent(this, troop));
        }

        public void ApplyEventEffects(Troop self)
        {
            if (self != null && !self.Destroyed && (!Happened || Repeatable))
            {
                Troop troopByPositionNoCheck;
                Happened = true;

                var list = new List<Troop>();
                if (SelfEffects.Count > 0)
                {
                    list.Add(self);
                    foreach (var effect in SelfEffects)
                    {
                        effect.ApplyEffect(self.Leader);
                    }
                }

                foreach (var person in effectPersons)
                {
                    var effectPerson = person.EffectPerson;
                    var locationTroop = effectPerson.LocationTroop;

                    person.Effect.ApplyEffect(effectPerson);
                    if (locationTroop != null && !list.Contains(locationTroop))
                    {
                        list.Add(locationTroop);
                    }
                }

                var list2 = new List<TroopEffectArea>();
                var list3 = new List<TroopEffectArea>();
                var list4 = new List<TroopEffectArea>();

                foreach (var area in effectAreas)
                {
                    switch (area.Kind)
                    {
                        case EffectAreaKind.视野敌军:
                            list2.Add(area);
                            break;

                        case EffectAreaKind.视野友军:
                            list2.Add(area);
                            break;

                        case EffectAreaKind.八格敌军:
                            list3.Add(area);
                            break;

                        case EffectAreaKind.八格友军:
                            list3.Add(area);
                            break;

                        case EffectAreaKind.攻击范围敌军:
                            list4.Add(area);
                            break;

                        case EffectAreaKind.攻击范围友军:
                            list4.Add(area);
                            break;
                    }
                }
                foreach (TroopEffectArea area in list2)
                {
                    foreach (Point point in self.BaseViewArea.Area)
                    {
                        if (self.BelongedFaction.IsPositionKnown(point))
                        {
                            troopByPositionNoCheck = Session.Current.Scenario.GetTroopByPositionNoCheck(point);
                            if (troopByPositionNoCheck != null)
                            {
                                switch (area.Kind)
                                {
                                    case EffectAreaKind.视野敌军:
                                        if (!self.IsFriendly(troopByPositionNoCheck.BelongedFaction))
                                        {
                                            area.Effect.ApplyEffect(troopByPositionNoCheck.Leader);
                                        }
                                        break;

                                    case EffectAreaKind.视野友军:
                                        if (self.IsFriendly(troopByPositionNoCheck.BelongedFaction))
                                        {
                                            area.Effect.ApplyEffect(troopByPositionNoCheck.Leader);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                foreach (TroopEffectArea area in list3)
                {
                    foreach (Point point in GameArea.GetArea(self.Position, 1, true).Area)
                    {
                        if (self.BelongedFaction.IsPositionKnown(point))
                        {
                            troopByPositionNoCheck = Session.Current.Scenario.GetTroopByPositionNoCheck(point);
                            if (troopByPositionNoCheck != null)
                            {
                                switch (area.Kind)
                                {
                                    case EffectAreaKind.八格敌军:
                                        if (!self.IsFriendly(troopByPositionNoCheck.BelongedFaction))
                                        {
                                            area.Effect.ApplyEffect(troopByPositionNoCheck.Leader);
                                        }
                                        break;

                                    case EffectAreaKind.八格友军:
                                        if (self.IsFriendly(troopByPositionNoCheck.BelongedFaction))
                                        {
                                            area.Effect.ApplyEffect(troopByPositionNoCheck.Leader);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                foreach (TroopEffectArea area in list4)
                {
                //Label_0539:
                    foreach (Point point in self.OffenceArea.Area)
                    {
                        if (!self.BelongedFaction.IsPositionKnown(point))
                        {
                            continue;
                        }
                        troopByPositionNoCheck = Session.Current.Scenario.GetTroopByPositionNoCheck(point);
                        if (troopByPositionNoCheck != null)
                        {
                            switch (area.Kind)
                            {
                                case EffectAreaKind.攻击范围敌军:
                                    if (!self.IsFriendly(troopByPositionNoCheck.BelongedFaction))
                                    {
                                        area.Effect.ApplyEffect(troopByPositionNoCheck.Leader);
                                    }
                                    break;

                                case EffectAreaKind.攻击范围友军:
                                    if (self.IsFriendly(troopByPositionNoCheck.BelongedFaction))
                                    {
                                        area.Effect.ApplyEffect(troopByPositionNoCheck.Leader);
                                    }
                                    break;
                            }
                        }
                    }
                }
                
                foreach (var troop in list)
                {
                    Troop.CheckTroopRout(troop);
                }
            }
        }

        public bool CheckCondition(Troop troop)
        {
            return Condition.CheckConditionList(Conditions, troop);
        }

        public bool CheckTroop(Troop troop)
        {
            if (!this.Happened || this.Repeatable)
            {
                if (!((this.AfterHappenedEvent == null) || this.AfterHappenedEvent.Happened))
                {
                    return false;
                }
                if (!GameObject.GetChance(this.HappenChance))
                {
                    return false;
                }
                if ((this.LaunchPerson == null) || troop.Persons.HasGameObject(this.LaunchPerson))
                {
                    if (!CheckCondition(troop)) return false;

                    if (TargetPersons.Count <= 0) return true;

                    GameArea baseViewArea = null;
                    switch (this.CheckArea)
                    {
                        case EventCheckAreaKind.Vision:
                            baseViewArea = troop.BaseViewArea;
                            break;

                        case EventCheckAreaKind.EightAdjacentTiles:
                            baseViewArea = GameArea.GetArea(troop.Position, 1, true);
                            break;

                        case EventCheckAreaKind.AttackRange:
                            baseViewArea = troop.OffenceArea;
                            break;
                    }
                    if (baseViewArea == null || troop.BelongedFaction == null) return false;
                    
                    int num = 0;
                    foreach (var point in baseViewArea.Area)
                    {
                        if (!troop.BelongedFaction.IsPositionKnown(point)) continue;
                        
                        var troopByPositionNoCheck = Session.Current.Scenario.GetTroopByPositionNoCheck(point);
                        if (troopByPositionNoCheck == null) continue;
                        
                        foreach (var relation in TargetPersons)
                        {
                            if (relation.Relation == PersonRelationKind.友好 
                                && troop.IsFriendly(troopByPositionNoCheck.BelongedFaction) 
                                && troopByPositionNoCheck.Persons.HasGameObject(relation.SpeakingPerson))
                            {
                                num++;
                            }
                        }
                    }
                    return num == TargetPersons.Count;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.ID;
        }

        public void LoadDialogFromString(Dictionary<int, Person> persons, string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            var strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            this.Dialogs.Clear();
            for (int i = 0; i < strArray.Length; i += 2)
            {
                PersonDialog item = new PersonDialog();
                int num2 = int.Parse(strArray[i]);
                if (num2 >= 0 && !persons.ContainsKey(num2)) continue;
                if (num2 >= 0)
                {
                    item.SpeakingPerson = persons[num2];
                    item.SpeakingPersonID = num2;
                } else
                {
                    item.SpeakingPerson = null;
                    item.SpeakingPersonID = -1;
                }
                item.Text = strArray[i + 1];
                Dialogs.Add(item);
            }
        }

        public void LoadEffectAreaFromString(Dictionary<int, EventEffect> eventEffects, string data)
        {
            var troopEffectAreas = new List<TroopEffectArea>();

            var strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strArray.Length; i += 2)
            {
                var eventEffectId = int.Parse(strArray[i + 1]);

                if (eventEffects.TryGetValue(eventEffectId, out var eventEffect))
                {
                    troopEffectAreas.Add(new TroopEffectArea
                    {
                        Kind = (EffectAreaKind)int.Parse(strArray[i]),
                        Effect = eventEffect,
                    });
                }
            }

            effectAreas = troopEffectAreas;
        }

        public void LoadEffectPersonFromString(Dictionary<int, Person> persons, Dictionary<int, EventEffect> eventEffects, string data)
        {
            var troopEffectPersons = new List<TroopEffectPerson>();

            var strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strArray.Length; i += 2)
            {
                var personId = int.Parse(strArray[i]);
                var eventEffectId = int.Parse(strArray[i + 1]);

                if (persons.TryGetValue(personId, out var person) && eventEffects.TryGetValue(eventEffectId, out var eventEffect))
                {
                    troopEffectPersons.Add(new TroopEffectPerson
                    {
                        EffectPerson = person,
                        Effect = eventEffect,
                    });
                }
            }

            effectPersons = troopEffectPersons;
        }

        public void LoadTargetPersonFromString(Dictionary<int, Person> persons, string data)
        {
            var personRelations = new List<PersonRelation>();

            var strArray = data.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strArray.Length; i += 2)
            {
                int personId = int.Parse(strArray[i + 1]);

                if (persons.TryGetValue(personId, out var person))
                {
                    personRelations.Add(new PersonRelation
                    {
                        SpeakingPerson = person,
                        Relation = (PersonRelationKind)int.Parse(strArray[i])
                    });
                }
            }

            TargetPersons = personRelations;
        }

        public string SaveDialogToString()//剧本的部队事件的对话武将默认全部变成了0，而且目前源码转换中，并没有这一块的安排
        {
            var sb = new StringBuilder();
            foreach (var dialog in Dialogs)
            {
                int personId = dialog.SpeakingPerson?.ID ?? -1;
                sb.Append(personId).Append(' ').Append(dialog.Text).Append(' ');
            }
            return sb.ToString();
        }

        public string SaveEffectAreaToString()
        {
            var sb = new StringBuilder();
            foreach (var area in effectAreas)
            {
                sb.Append((int)area.Kind).Append(' ').Append(area.Effect.ID).Append(' ');
            }
            return sb.ToString();
        }

        public string SaveEffectPersonToString()
        {
            var sb = new StringBuilder();
            foreach (var person in effectPersons)
            {
                sb.Append(person.EffectPerson.ID).Append(' ').Append(person.Effect.ID).Append(' ');
            }
            return sb.ToString();
        }

        public string SaveTargetPersonToString()
        {
            var sb = new StringBuilder();
            foreach (var relation in TargetPersons)
            {
                sb.Append(relation.Relation).Append(' ').Append(relation.SpeakingPerson.ID).Append(' ');
            }
            return sb.ToString();
        }
    }
}
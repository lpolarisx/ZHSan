using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using GameManager;
using GameEnums;
using System.Linq;
using GameObjects.PersonDetail;
using System.Collections.Generic;
using GameEvents;
using GameGlobal;
using GameDatas;

namespace GameObjects
{
    [DataContract]
    public class Captive : GameObject
    {
        private EventManager eventManager = EventManager.Instance;

        public Faction CaptiveFaction => Session.Current.Scenario.Factions.GetValueOrDefault(CaptiveFactionID);

        [DataMember]
        public int CaptiveFactionID { get; set; }

        public Person CaptivePerson => Session.Current.Scenario.AllPersons.GetValueOrDefault(CaptivePersonID);

        [DataMember]
        public int CaptivePersonID { get; set; }

        public Architecture RansomArchitecture;

        [DataMember]
        public int RansomArchitectureID { get; set; }

        [DataMember]
        public int RansomArriveDays { get; set; }

        public Captive() {}

        public Captive(CaptiveConfig config)
        {
            ID = config.Id;
            CaptiveFactionID = config.CaptiveFactionId;
            CaptivePersonID = config.CaptivePersonId;
            RansomArchitectureID = config.RansomArchitectureId;
            RansomArriveDays = config.RansomArriveDays;
            RansomFund = config.RansomFund;
        }

        public CaptiveConfig ToConfig()
        {
            return new CaptiveConfig
            {
                Id = ID,
                CaptiveFactionId = CaptiveFactionID,
                CaptivePersonId = CaptivePersonID,
                RansomArchitectureId = RansomArchitectureID,
                RansomArriveDays = RansomArriveDays,
                RansomFund = RansomFund,
            };
        }

        public void ClearEvents()
        {
        }

        public static Captive Create(Person person, Faction capturingFaction)
        {
            if (person.BelongedFaction == capturingFaction) return null;

            var captive = new Captive
            {
                ID = Session.Current.Scenario.GetCaptives().Max(x => x.ID) + 1,
                CaptivePersonID = person.ID,
                CaptiveFactionID = person.BelongedFaction.ID
            };

            person.DecreaseReputation(50);
            person.SetBelongedCaptive(captive, PersonStatus.Captive);
            person.HeldCaptiveCount++;
            // Session.Current.Scenario.Captives.AddCaptiveWithEvent(captive);

            return captive;
        }

        public Faction BelongedFaction
        {
            get
            {
                if (this.CaptivePerson.LocationArchitecture != null)
                {
                    return this.CaptivePerson.LocationArchitecture.BelongedFaction;
                }
                else if (this.CaptivePerson.LocationTroop != null)
                {
                    return this.CaptivePerson.LocationTroop.BelongedFaction;
                }
                else
                {
                    // this should not happen...
                    return null;
                }
            }
        }

        public Architecture LocationArchitecture => CaptivePerson.LocationArchitecture;

        public Troop LocationTroop => CaptivePerson.LocationTroop;

        public void DayEvent()
        {
            if (((this.BelongedFaction != null) && (this.CaptiveFaction != null)) && (this.RansomArriveDays > 0))
            {
                //this.RansomArriveDays--;
                this.RansomArriveDays -= Session.Parameters.DayInTurn;
                if (this.RansomArriveDays == 0)
                {
                    if (this.BelongedFaction.Capital != null)
                    {
                        if (this.BelongedFaction.Capital == this.RansomArchitecture)
                        {
                            if (Session.Current.Scenario.IsPlayer(this.BelongedFaction))
                            {
                                if (BelongedFaction.AutoRefuse)
                                {
                                    ReturnRansom();
                                }

                                eventManager.Publish(new PlayerReleaseEvent(BelongedFaction, CaptiveFaction, this));

                                // Session.MainGame.mainGameScreen.CaptivePlayerRelease(BelongedFaction, CaptiveFaction, this);
                            }
                            else
                            {
                                int diplomaticRelation = Session.Current.Scenario.GetDiplomaticRelation(this.BelongedFaction.ID, this.CaptiveFaction.ID);
                                //if (((diplomaticRelation >= 0) || (GameObject.Random(this.RansomFund) > GameObject.Random(this.RansomFund + this.BelongedFaction.Capital.Fund))) || (GameObject.Random(Math.Abs(diplomaticRelation) + 100) < GameObject.Random(100)))
                                if (diplomaticRelation >= 0 || ((!this.CaptivePerson.RecruitableBy(this.BelongedFaction, 0) || (this.CaptivePerson.Loyalty >= 100 && GameObject.GetChance(80 - this.CaptivePerson.PersonalLoyalty * 20))) 
                                    && (!this.BelongedFaction.Capital.IsFundEnough || GameObject.Random(this.RansomFund) > GameObject.Random(this.RansomFund + this.BelongedFaction.Capital.Fund)))
                                    && (!Session.GlobalVariables.AIAutoTakePlayerCaptives))
                                {
                                    this.ReleaseCaptive();
                                }
                                else
                                {
                                    this.ReturnRansom();
                                }
                            }
                        }
                        else
                        {
                            //this.RansomArriveDays = (int) (Session.Current.Scenario.GetDistance(this.RansomArchitecture.ArchitectureArea, this.BelongedFaction.Capital.ArchitectureArea) / 5.0);
                            this.RansomArriveDays = (int)(Session.Current.Scenario.GetDistance(this.RansomArchitecture.ArchitectureArea, this.BelongedFaction.Capital.ArchitectureArea) / 5.0) * Session.Parameters.DayInTurn;
                            if (this.RansomArriveDays <= 0)
                            {
                                this.RansomArriveDays = 1;
                            }
                            this.RansomArchitecture = this.BelongedFaction.Capital;
                        }
                    }
                    else
                    {
                        this.ReturnRansom();
                    }
                }
            }
        }

        private void DoRelease()
        {
            Point position = CaptivePerson.Position;
            if (this.CaptivePerson.BelongedFaction != null && this.CaptivePerson.BelongedFaction.Capital != null)
            {
                Faction f = this.CaptivePerson.BelongedFaction;
                this.CaptivePerson.LocationArchitecture = f.Capital;
                this.CaptivePerson.Status = GameObjects.PersonDetail.PersonStatus.Normal;
                this.CaptivePerson.MoveToArchitecture(f.Capital, position, false, true, null);
            }
            
            else
            {

                this.TransformToNoFaction();
                return;
                
            }
            this.Clear();
        }

        public void Clear()
        {
            this.CaptivePerson.SetBelongedCaptive(null, this.CaptivePerson.ArrivingDays > 0 ? GameObjects.PersonDetail.PersonStatus.Moving : GameObjects.PersonDetail.PersonStatus.Normal);
            this.RansomArchitecture = null;
            this.RansomFund = 0;
        }

        private void DoReturn()
        {
            if ((this.CaptiveFaction!=null) && ((this.CaptiveFaction.Capital != null)) && (this.BelongedFaction != null) && (this.BelongedFaction.Capital != null))
            {
                //int num = (int) (Session.Current.Scenario.GetDistance(this.CaptiveFaction.Capital.ArchitectureArea, this.BelongedFaction.Capital.ArchitectureArea) / 5.0);
                int num = (int) (Session.Current.Scenario.GetDistance(this.CaptiveFaction.Capital.ArchitectureArea, this.BelongedFaction.Capital.ArchitectureArea) / 5.0) * Session.Parameters.DayInTurn;
                if (num <= 0)
                {
                    num = 1;
                }
                this.CaptiveFaction.Capital.AddFundPack(this.RansomFund, this.RansomArriveDays + num);
            }
        }

        public void ReleaseCaptive()
        {
            eventManager.Publish(new ReleaseEvent(true, BelongedFaction, CaptiveFaction, CaptivePerson));
            RansomArchitecture.IncreaseFund(RansomFund);
            DoRelease();
        }

        public void ReturnRansom()
        {
            eventManager.Publish(new ReleaseEvent(false, BelongedFaction, CaptiveFaction, CaptivePerson));
            DoReturn();
        }

        public void SelfReleaseCaptive()
        {
            eventManager.Publish(new SelfReleaseEvent(this));

            if (BelongedFaction != null && CaptiveFaction != null)
            {
                Session.Current.Scenario.ChangeDiplomaticRelation(BelongedFaction.ID, CaptiveFaction.ID, ReleaseRelation / 400);
            }
            if (GameObject.GetChance(CaptivePerson.Karma + CaptivePerson.PersonalLoyalty * 10))
            {
                BelongedFaction.Leader.IncreaseKarma(1);
            }
            DoReturn();
            DoRelease();
        }

        public void CaptiveEscape()
        {
            eventManager.Publish(new EscapeEvent(this));
            CaptivePerson.FleeCount++;

            if (BelongedFaction != null)
            {
                Session.MainGame.mainGameScreen.xianshishijiantupian(CaptivePerson, BelongedFaction.Name, TextMessageKind.CaptiveEscape, "CaptiveEscape", "", "", false);
            }

            DoReturn();
            DoRelease();
        }

        public void CaptiveEscapeNoHint()
        {
            this.CaptivePerson.FleeCount++;
            this.DoReturn();
            this.DoRelease();
        }

        public void CaptiveDirectEscape()
        {
            this.DoReturn();
            this.DoRelease();
        }

        public void SendRansom(Architecture to, Architecture from)
        {
            this.RansomFund = this.Ransom;
            from.DecreaseFund(this.RansomFund);
            this.RansomArchitecture = to;
            //this.RansomArriveDays = (int) (Session.Current.Scenario.GetDistance(from.ArchitectureArea, to.ArchitectureArea) / 5.0);
            this.RansomArriveDays = (int)(Session.Current.Scenario.GetDistance(from.ArchitectureArea, to.ArchitectureArea) / 5.0) * Session.Parameters.DayInTurn;
            if (this.RansomArriveDays <= 0)
            {
                this.RansomArriveDays = 1;
            }
        }

        public void TransformToNoFactionCaptive()
        {
            if (CaptivePerson != null && CaptivePerson.BelongedFaction != null)
            {
                CaptiveFactionID = -1;
            }
        }

        // 变成在野人物
        public void TransformToNoFaction()  
        {
            if (CaptivePerson == null) return;

            CaptivePerson.Status = PersonStatus.NoFaction;

            if (LocationTroop != null)
            {
                CaptivePerson.LocationArchitecture = StaticMethods.GetRandomItem(Session.Current.Scenario.Architectures.Values.ToList());

                var architecture = CaptivePerson.BelongedFaction != null && BelongedFaction.Capital != null
                                   ? BelongedFaction.Capital
                                   : StaticMethods.GetRandomItem(Session.Current.Scenario.Architectures.Values.ToList());

                CaptivePerson.MoveToArchitecture(architecture, CaptivePerson.LocationTroop.Position, false, true, null);
            }
            
            CaptivePerson.SetBelongedCaptive(null, PersonStatus.NoFaction);
        }

        public string BelongedFactionString => BelongedFaction?.Name ?? "----";

        public string CaptiveFactionString => CaptiveFaction?.Name ?? "----";

        public string LocationString
        {
            get
            {
                if (!Session.Current.Scenario.IsCurrentPlayer(CaptiveFaction) || Session.GlobalVariables.SkyEye)
                {
                    if (LocationArchitecture != null)
                    {
                        return LocationArchitecture.Name;
                    }
                    if (LocationTroop != null)
                    {
                        return LocationTroop.DisplayName;
                    }
                }
                return "----";
            }
        }

        public int Loyalty => CaptivePerson?.Loyalty ?? 100;

        public string LoyaltyString
        {
            get
            {
                if (this.CaptivePerson != null)
                {
                    if (Session.Current.Scenario.IsCurrentPlayer(this.CaptiveFaction))
                    {
                        return "----";
                    }
                    return this.CaptivePerson.Loyalty.ToString();
                }
                return "----";
            }
        }

        public string Travel
        {
            get
            {
                if (CaptivePerson != null && CaptivePerson.LocationTroop == null)
                {
                    if (CaptivePerson.ArrivingDays > 0)
                    {
                        return $"{CaptivePerson.ArrivingDays * Session.Current.Scenario.Parameters.DayInTurn}天";
                    }
                }
                
                return "----";
            }
        }

        public string RansomArriveDaysString => CaptivePerson != null && RansomArriveDays > 0 ? $"{RansomArriveDays}天" : "----";
       
        public new string Name => CaptivePerson?.Name ?? "----";

        public int Ransom
        {
            get
            {
                if (this.CaptivePerson != null)
                {
                    return (int)(this.CaptivePerson.UntiredMerit * Session.Parameters.RansomRate);
                    //return 10 * (int)(((float)((this.CaptivePerson.UntiredMerit * ((this.CaptiveFaction.Leader == this.CaptivePerson) ? 2 : 1)) / 50)) ));
                }
                return 0;
            }
        }

        [DataMember]
        public int RansomFund { get; set; }

        public int ReleaseRelation
        {
            get
            {
                if (this.CaptivePerson != null)
                {
                    return (int) (((this.CaptivePerson.Merit * ((this.CaptiveFaction.Leader == this.CaptivePerson) ? 2 : 1)) / 50) * ((this.CaptiveFaction != null) ? Math.Pow((double) this.CaptiveFaction.InternalSurplusRate, 1.7999999523162842) : 1.0));
                }
                return 0;
            }
        }

        public int AIWantsTheCaptive
        {
            get
            {
                if (!CaptivePerson.WillLoseLoyaltyWhenHeldCaptive)
                {
                    return CaptivePerson.Merit / 2;
                }

                return CaptivePerson.Merit + (110 - Loyalty) * 500 + (LocationArchitecture.noEscapeChance - CaptivePerson.captiveEscapeChance) * 300;
            }
        }

        public int Merit => CaptivePerson.Merit;

        public delegate void PlayerRelease(Faction from, Faction to, Captive captive);

        public delegate void Release(bool success, Faction from, Faction to, Person person);

        public delegate void SelfRelease(Captive captive);

        public delegate void Escape(Captive captive);
    }
}
using GameDatas;
using GameManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects.FactionDetail
{
    /// <summary>
    /// 一份外交条款：提议方给出什么、索要什么，以及双方共同的停战、结盟。
    /// 亲善（只给钱）、停战、结盟、割地都是它的一种组合
    /// </summary>
    public class DiplomacyDeal
    {
        /// <summary>
        /// 停战可选的天数，0表示不停战
        /// </summary>
        public static readonly int[] TruceOptions = { 0, 90, 180, 360 };

        public Faction Source;
        public Faction Target;

        /// <summary>
        /// 提议方出钱粮的城池，对方给的钱粮也运到这里
        /// </summary>
        public Architecture SourceArchitecture;

        /// <summary>
        /// 提议方的使者，影响对方的态度，可以派几个人同去。对方遣使来访时为空
        /// </summary>
        public List<Person> Envoys = new List<Person>();

        public int GiveFund;
        public int GiveFood;
        public List<Architecture> GiveArchitectures = new List<Architecture>();
        public int RequestFund;
        public int RequestFood;
        public List<Architecture> RequestArchitectures = new List<Architecture>();
        public int TruceDays;
        public bool Alliance;

        public DiplomacyDeal(Faction source, Faction target, Architecture sourceArchitecture)
        {
            this.Source = source;
            this.Target = target;
            this.SourceArchitecture = sourceArchitecture;
        }

        /// <summary>
        /// 主使：政治最高的那位，带着条款去交割，其余是随行
        /// </summary>
        public Person LeadEnvoy => this.Envoys.Count == 0 ? null : this.Envoys.MaxBy(x => x.Politics);

        public bool IsEmpty =>
            GiveFund == 0 && GiveFood == 0 && GiveArchitectures.Count == 0 &&
            RequestFund == 0 && RequestFood == 0 && RequestArchitectures.Count == 0 &&
            TruceDays == 0 && !Alliance;

        /// <summary>
        /// 换成对方的立场：对方给出的变成索要的。用于玩家对AI的来使还价
        /// </summary>
        /// <param name="sourceArchitecture">换过来之后提议方出钱粮的城池</param>
        public DiplomacyDeal Reverse(Architecture sourceArchitecture)
        {
            return new DiplomacyDeal(this.Target, this.Source, sourceArchitecture)
            {
                GiveFund = this.RequestFund,
                GiveFood = this.RequestFood,
                GiveArchitectures = new List<Architecture>(this.RequestArchitectures),
                RequestFund = this.GiveFund,
                RequestFood = this.GiveFood,
                RequestArchitectures = new List<Architecture>(this.GiveArchitectures),
                TruceDays = this.TruceDays,
                Alliance = this.Alliance
            };
        }

        public DiplomacyDealConfig ToConfig()
        {
            return new DiplomacyDealConfig
            {
                SourceFactionID = Source.ID,
                TargetFactionID = Target.ID,
                SourceArchitectureID = SourceArchitecture?.ID ?? -1,
                CompanionIDs = Envoys.Where(x => x != LeadEnvoy).Select(x => x.ID).ToList(),
                GiveFund = GiveFund,
                GiveFood = GiveFood,
                GiveArchitectureIDs = GiveArchitectures.Select(x => x.ID).ToList(),
                RequestFund = RequestFund,
                RequestFood = RequestFood,
                RequestArchitectureIDs = RequestArchitectures.Select(x => x.ID).ToList(),
                TruceDays = TruceDays,
                Alliance = Alliance
            };
        }

        /// <summary>
        /// 从存档恢复。提议方或接受方已经灭亡时返回null
        /// </summary>
        /// <param name="envoy">带着条款的主使，随行的使者从存档里找回</param>
        public static DiplomacyDeal FromConfig(DiplomacyDealConfig config, Person envoy)
        {
            var scenario = Session.Current.Scenario;
            Faction source = scenario.Factions.GetValueOrDefault(config.SourceFactionID);
            Faction target = scenario.Factions.GetValueOrDefault(config.TargetFactionID);
            if (source == null || target == null)
            {
                return null;
            }
            // 城池已不存在的就略过，交割前的检查会发现条款变了
            return new DiplomacyDeal(source, target, scenario.Architectures.GetValueOrDefault(config.SourceArchitectureID))
            {
                Envoys = new List<Person> { envoy }.Concat(config.CompanionIDs.Select(x => scenario.AllPersons.GetValueOrDefault(x)).Where(x => x != null && x != envoy)).ToList(),
                GiveFund = config.GiveFund,
                GiveFood = config.GiveFood,
                GiveArchitectures = config.GiveArchitectureIDs.Select(x => scenario.Architectures.GetValueOrDefault(x)).Where(x => x != null).ToList(),
                RequestFund = config.RequestFund,
                RequestFood = config.RequestFood,
                RequestArchitectures = config.RequestArchitectureIDs.Select(x => scenario.Architectures.GetValueOrDefault(x)).Where(x => x != null).ToList(),
                TruceDays = config.TruceDays,
                Alliance = config.Alliance
            };
        }

        /// <summary>
        /// 提议方能割让的城池：除都城和出钱粮的城以外的城。出钱粮的城是使者的归处，不能割
        /// </summary>
        public List<Architecture> GetGivableArchitectures()
        {
            var result = new List<Architecture>();
            foreach (var a in this.Source.Architectures)
            {
                if (a != this.Source.Capital && a != this.SourceArchitecture)
                {
                    result.Add(a);
                }
            }
            return result;
        }

        /// <summary>
        /// 能向对方索要的城池：对方都城以外的城
        /// </summary>
        public List<Architecture> GetRequestableArchitectures()
        {
            var result = new List<Architecture>();
            foreach (var a in this.Target.Architectures)
            {
                if (a != this.Target.Capital)
                {
                    result.Add(a);
                }
            }
            return result;
        }

        /// <summary>
        /// 条款现在能不能交割，不能时给出原因。谈的时候和使者到达时都要检查
        /// </summary>
        /// <param name="sourcePaid">提议方的钱粮已经在使者出发时扣过了</param>
        public string GetInvalidReason(bool sourcePaid)
        {
            if (this.Source.Capital == null || this.Target.Capital == null)
            {
                return "一方已无都城。";
            }
            if (this.IsEmpty)
            {
                return "条款是空的。";
            }
            if (!sourcePaid && this.SourceArchitecture != null)
            {
                if (this.GiveFund > this.SourceArchitecture.Fund)
                {
                    return this.SourceArchitecture.Name + "资金不足。";
                }
                if (this.GiveFood > this.SourceArchitecture.Food)
                {
                    return this.SourceArchitecture.Name + "粮草不足。";
                }
            }
            var givable = this.GetGivableArchitectures();
            foreach (var a in this.GiveArchitectures)
            {
                if (!givable.Contains(a))
                {
                    return a.Name + "已无法割让。";
                }
            }
            var requestable = this.GetRequestableArchitectures();
            foreach (var a in this.RequestArchitectures)
            {
                if (!requestable.Contains(a))
                {
                    return a.Name + "已不在对方手中。";
                }
            }
            if (this.RequestFund > this.Target.Capital.Fund)
            {
                return "对方都城资金不足。";
            }
            if (this.RequestFood > this.Target.Capital.Food)
            {
                return "对方都城粮草不足。";
            }
            var relation = this.GetRelation();
            if (relation == null)
            {
                return "双方没有外交关系。";
            }
            if (this.Alliance)
            {
                if (relation.Relation >= Session.GlobalVariables.FriendlyDiplomacyThreshold)
                {
                    return "双方已经结盟。";
                }
                if (relation.AllianceCooldown > 0)
                {
                    return "双方解除同盟不久，" + relation.AllianceCooldown + "个月内不能再结盟。";
                }
                if (relation.Relation < DiplomacyEvaluator.AllyRelation)
                {
                    return "双方关系需达到 " + DiplomacyEvaluator.AllyRelation + " 才能结盟。";
                }
            }
            if (this.TruceDays > 0 && relation.Truce >= this.TruceDays)
            {
                return "双方停战期还未到。";
            }
            return null;
        }

        public DiplomaticRelation GetRelation()
        {
            Session.Current.Scenario.TryGetDiplomaticRelation(this.Source.ID, this.Target.ID, out var relation);
            return relation;
        }

        /// <summary>
        /// 使者出发时先扣下提议方的钱粮，由使者带去
        /// </summary>
        public void PaySource()
        {
            this.SourceArchitecture.DecreaseFund(this.GiveFund);
            this.SourceArchitecture.DecreaseFood(this.GiveFood);
        }

        /// <summary>
        /// 谈不成时退回提议方的钱粮
        /// </summary>
        public void RefundSource()
        {
            Refund(this.Source, this.SourceArchitecture, this.GiveFund, this.GiveFood);
        }

        /// <summary>
        /// 按存档里的条款退回提议方的钱粮。只要提议方还在就退，接受方灭亡了也不影响
        /// </summary>
        public static void RefundSource(DiplomacyDealConfig config)
        {
            var scenario = Session.Current.Scenario;
            Faction source = scenario.Factions.GetValueOrDefault(config.SourceFactionID);
            if (source != null)
            {
                Refund(source, scenario.Architectures.GetValueOrDefault(config.SourceArchitectureID), config.GiveFund, config.GiveFood);
            }
        }

        // 退回出钱粮的城，那座城已经丢了就退到都城
        private static void Refund(Faction source, Architecture sourceArchitecture, int fund, int food)
        {
            Architecture a = sourceArchitecture != null && sourceArchitecture.BelongedFaction == source ? sourceArchitecture : source.Capital;
            if (a != null)
            {
                a.IncreaseFund(fund);
                a.IncreaseFood(food);
            }
        }

        /// <summary>
        /// 交割：提议方的钱粮由使者当场交给对方都城，对方的钱粮按路程运到提议方的城池
        /// </summary>
        /// <param name="sourcePaid">提议方的钱粮已经在使者出发时扣过了</param>
        /// <returns>交割结果的简述：索要的钱粮几日后运到哪里、双方关系的变化</returns>
        public string Execute(bool sourcePaid)
        {
            int relationGain = DiplomacyEvaluator.GetRelationGain(this);
            var parts = new List<string>();
            Architecture receiver = this.SourceArchitecture != null && this.SourceArchitecture.BelongedFaction == this.Source ? this.SourceArchitecture : this.Source.Capital;

            if (!sourcePaid)
            {
                this.PaySource();
            }
            this.Target.Capital.IncreaseFund(this.GiveFund);
            this.Target.Capital.IncreaseFood(this.GiveFood);

            if (this.RequestFund > 0 || this.RequestFood > 0)
            {
                int days = Math.Max(1, Session.Current.Scenario.GetTransferFundDays(this.Target.Capital, receiver));
                this.Target.Capital.DecreaseFund(this.RequestFund);
                this.Target.Capital.DecreaseFood(this.RequestFood);
                if (this.RequestFund > 0)
                {
                    receiver.AddFundPack(this.RequestFund, days);
                }
                if (this.RequestFood > 0)
                {
                    receiver.AddFoodPack(this.RequestFood, days);
                }
                var goods = new List<string>();
                if (this.RequestFund > 0) goods.Add("资金" + this.RequestFund.ToString("N0"));
                if (this.RequestFood > 0) goods.Add("粮草" + this.RequestFood.ToString("N0"));
                parts.Add(string.Join("、", goods) + "将于" + days + "日后运抵" + receiver.Name);
            }

            foreach (var a in this.GiveArchitectures)
            {
                this.Source.CedeArchitecture(a, this.Target);
            }
            foreach (var a in this.RequestArchitectures)
            {
                this.Target.CedeArchitecture(a, this.Source);
            }

            var relation = this.GetRelation();
            if (relation != null)
            {
                int before = relation.Relation;
                int threshold = Session.GlobalVariables.FriendlyDiplomacyThreshold;
                if (relationGain > 0)
                {
                    // 同亲善：送礼只能把关系提到接近同盟，结盟要另外谈
                    int cap = relation.Relation >= threshold ? int.MaxValue : (int)(threshold * 0.95);
                    relation.Relation = Math.Max(relation.Relation, Math.Min(relation.Relation + relationGain, cap));
                }
                else if (relationGain < 0)
                {
                    // 对方吃了亏（被索贡、割地求停战等），答应了心里也不痛快
                    relation.Worsen(-relationGain);
                }
                if (this.Alliance)
                {
                    relation.Relation = Math.Max(relation.Relation, threshold);
                }
                int delta = relation.Relation - before;
                parts.Add("双方关系" + (delta > 0 ? "+" + delta : delta < 0 ? delta.ToString() : "不变") + "（现为" + relation.Relation + "）");
                // 结盟附带半年停战，免得刚结盟就被解除
                int truceDays = this.Alliance ? Math.Max(this.TruceDays, DiplomacyEvaluator.AllianceTruceDays) : this.TruceDays;
                if (truceDays > relation.Truce)
                {
                    relation.Truce = truceDays;
                }
                relation.NegotiationCount = 0;
            }

            this.Source.Leader.AdjustRelation(this.Target.Leader, 6f, 2);
            this.Target.Leader.AdjustRelation(this.Source.Leader, 6f, 2);
            // 主使全赏，随行的减半；途中身亡或已离开本势力的不赏
            Person lead = this.LeadEnvoy;
            foreach (var envoy in this.Envoys)
            {
                if (!envoy.Alive || envoy.BelongedFaction != this.Source) continue;

                int share = envoy == lead ? 2 : 1;
                envoy.AddPoliticsExperience(5 * share);
                envoy.IncreaseReputation(100 * share);
                envoy.IncreaseOfficerMerit(50 * share);
            }
            return string.Join("；", parts) + "。";
        }

        /// <summary>
        /// 一方的条款列成一行，例如“资金 50,000、停战 6个月”
        /// </summary>
        /// <param name="giving">true列提议方给出的，false列提议方索要的</param>
        public string DescribeSide(bool giving)
        {
            var parts = new List<string>();
            int fund = giving ? this.GiveFund : this.RequestFund;
            int food = giving ? this.GiveFood : this.RequestFood;
            List<Architecture> architectures = giving ? this.GiveArchitectures : this.RequestArchitectures;
            if (fund > 0)
            {
                parts.Add("资金 " + fund.ToString("N0"));
            }
            if (food > 0)
            {
                parts.Add("粮草 " + food.ToString("N0"));
            }
            if (architectures.Count > 0)
            {
                parts.Add("城池 " + string.Join("、", architectures.Select(x => x.Name)));
            }
            return parts.Count == 0 ? "无" : string.Join("、", parts);
        }

        /// <summary>
        /// 双方共同的条款，例如“停战 6个月、结盟”
        /// </summary>
        public string DescribeMutual()
        {
            var parts = new List<string>();
            if (this.TruceDays > 0)
            {
                parts.Add("停战 " + TruceText(this.TruceDays));
            }
            if (this.Alliance)
            {
                parts.Add("结盟");
            }
            return parts.Count == 0 ? "无" : string.Join("、", parts);
        }

        public static string TruceText(int days)
        {
            if (days <= 0)
            {
                return "无";
            }
            return days % 360 == 0 ? (days / 360) + "年" : (days / 30) + "个月";
        }
    }
}

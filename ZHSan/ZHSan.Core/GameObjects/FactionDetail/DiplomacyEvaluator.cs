using GameManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameObjects.FactionDetail
{
    public enum DiplomacyResponseKind
    {
        /// <summary>
        /// 接受
        /// </summary>
        Accept,

        /// <summary>
        /// 还价：再加些钱粮就接受
        /// </summary>
        Counter,

        /// <summary>
        /// 拒绝
        /// </summary>
        Reject,

        /// <summary>
        /// 本月谈得太多，不再接谈
        /// </summary>
        Refuse
    }

    /// <summary>
    /// 对方对一份条款的回应
    /// </summary>
    public class DiplomacyResponse
    {
        public DiplomacyResponseKind Kind;
        public string Text;
        // 还价时要求提议方再加的钱粮
        public int ExtraFund;
        public int ExtraFood;
    }

    /// <summary>
    /// 接受方给一份条款估值。所有价值都折算成资金：正数表示对接受方有利。
    /// 实利分成得到和付出两笔（钱粮城池、停战结盟的利弊）；情面（双方关系、使者的才能和与对方君主的亲疏）不直接折钱，
    /// 只决定对方肯让利多少：情面好时付出打折，情面差时付出加价。
    /// 关系好的势力还肯从余钱余粮里白给一些（援助），先抵掉索要的钱粮，提议方越缺越肯给
    /// </summary>
    public static class DiplomacyEvaluator
    {
        /// <summary>
        /// 每对势力每月最多谈不成几次
        /// </summary>
        public const int MaxNegotiationsPerMonth = 3;

        /// <summary>
        /// 结盟要求的最低关系
        /// </summary>
        /// <summary>
        /// 结盟时附带的停战天数，盟约期内AI不会解除同盟
        /// </summary>
        public const int AllianceTruceDays = 180;

        /// <summary>
        /// 解除同盟后多少个月内不能再结盟
        /// </summary>
        public const int AllianceCooldownMonths = 6;

        /// <summary>
        /// 解除同盟后，被背弃的一方与背盟者的关系
        /// </summary>
        public const int BetrayalRelation = -50;

        /// <summary>
        /// 解除同盟后，其他势力与背盟者的关系下降多少
        /// </summary>
        public const int BetrayalPenalty = 20;

        /// <summary>
        /// 解除同盟（背弃盟约）：被背弃的一方关系降到负数，君主声誉受损，一段时间内不能再结盟；
        /// 背盟的事传开，其他势力与背盟者的关系也下降，现有盟友只降不散。停战照旧，停战期内仍不能开战
        /// </summary>
        public static void BreakAlliance(Faction breaker, DiplomaticRelation relation)
        {
            relation.Relation = BetrayalRelation;
            relation.AllianceCooldown = AllianceCooldownMonths;
            breaker.Leader.DecreaseKarma(5);
            foreach (var other in Session.Current.Scenario.GetDiplomaticRelationListByFactionID(breaker.ID))
            {
                if (other != relation)
                {
                    other.Worsen(BetrayalPenalty);
                }
            }
        }

        public static int AllyRelation => (int)Math.Ceiling(Session.GlobalVariables.FriendlyDiplomacyThreshold * 0.9);

        // 一份资金值多少粮草：取买粮和卖粮价格的中间
        public static float FoodPerFund => (Session.Parameters.FundToFoodMultiple + Session.Parameters.FoodToFundDivisor) / 2f;

        /// <summary>
        /// 情面好时最多肯让利几成
        /// </summary>
        public const double MaxConcession = 0.3;

        /// <summary>
        /// 情面差时最多要加价几成
        /// </summary>
        public const double MaxPremium = 0.5;

        /// <summary>
        /// 多少情面折合一成让利
        /// </summary>
        public const int GoodwillPerTenPercent = 30000;

        /// <summary>
        /// 接受方的总估值：付出先扣掉肯白给的援助，剩下的按让利率折算，得到的减去它不小于0就会接受
        /// </summary>
        public static int Evaluate(DiplomacyDeal deal)
        {
            return (int)(GetGainAndRequired(deal, out double required) - required);
        }

        // 得到的，以及要得到多少才肯接受
        private static double GetGainAndRequired(DiplomacyDeal deal, out double required)
        {
            GetMaterialBalance(deal, out double gain, out double cost);
            required = Math.Max(0, cost - GetAidValue(deal)) * (1 - GetConcessionRate(deal));
            return gain;
        }

        /// <summary>
        /// 关系低于这个值时一点援助都不给
        /// </summary>
        public const int AidMinRelation = 50;

        /// <summary>
        /// 白拿多少援助折掉一点关系，比按实利算的吃亏（500一点）轻得多，人情用多了也会淡
        /// </summary>
        public const int AidValuePerRelation = 2000;

        /// <summary>
        /// 对方肯白给的援助，折算成对方付出的价值。只抵索要的钱粮，城池、停战、结盟不算。
        /// 关系越好越大方，到结盟线算足额，再好最多一倍半；提议方的城缺钱（缺粮）时肯拿出余钱（余粮）的一半，不缺时只给一成；
        /// 只从维持本城所需之外的余量里出。每月心情不同，同一个月里不变，免得反复提议碰运气
        /// </summary>
        public static double GetAidValue(DiplomacyDeal deal)
        {
            if (deal.RequestFund == 0 && deal.RequestFood == 0) return 0;

            var relation = deal.GetRelation();
            Architecture capital = deal.Target.Capital;
            Architecture receiver = deal.SourceArchitecture ?? deal.Source.Capital;
            if (relation == null || capital == null || receiver == null) return 0;

            double generosity = GetGenerosity(relation, deal.Source, deal.Target);
            if (generosity <= 0) return 0;

            int spareFund = Math.Max(0, capital.Fund - capital.EnoughFund);
            int spareFood = Math.Max(0, capital.Food - capital.EnoughFood);
            int fund = Math.Min(deal.RequestFund, (int)(spareFund * (receiver.IsFundEnough ? 0.1 : 0.5) * generosity));
            int food = Math.Min(deal.RequestFood, (int)(spareFood * (receiver.IsFoodEnough ? 0.1 : 0.5) * generosity));
            return fund * GetFundLossFactor(capital) + food / FoodPerFund * GetFoodLossFactor(capital);
        }

        /// <summary>
        /// 援助方这个月对受援方有多慷慨，0表示一点不给：关系越好越大方，到结盟线算1倍，最多一倍半；
        /// 再乘上这个月的心情0.6到1.4倍，按年月和双方算出，不用随机数，存读档也不变
        /// </summary>
        public static double GetGenerosity(DiplomaticRelation relation, Faction receiver, Faction giver)
        {
            double generosity = Math.Clamp((relation.Relation - AidMinRelation) / (double)(Session.GlobalVariables.FriendlyDiplomacyThreshold - AidMinRelation), 0, 1.5);
            var date = Session.Current.Scenario.Date;
            int seed = unchecked((date.Year * 12 + date.Month) * 7919 + receiver.ID * 131 + giver.ID * 31);
            return generosity * (0.6 + (seed & 0x7fffffff) % 81 / 100.0);
        }

        /// <summary>
        /// 实利净值：接受方得到的减去付出的，不计情面
        /// </summary>
        public static int GetMaterialValue(DiplomacyDeal deal)
        {
            GetMaterialBalance(deal, out double gain, out double cost);
            return (int)(gain - cost);
        }

        /// <summary>
        /// 实利分两笔：接受方得到的和付出的，都不小于0。停战结盟按利弊归入其中一笔
        /// </summary>
        public static void GetMaterialBalance(DiplomacyDeal deal, out double gain, out double cost)
        {
            Architecture capital = deal.Target.Capital;
            gain = 0;
            cost = 0;
            gain += deal.GiveFund * GetFundFactor(capital);
            gain += deal.GiveFood / FoodPerFund * GetFoodFactor(capital);
            cost += deal.RequestFund * GetFundLossFactor(capital);
            cost += deal.RequestFood / FoodPerFund * GetFoodLossFactor(capital);
            foreach (var a in deal.GiveArchitectures)
            {
                gain += GetArchitectureValue(a) * (IsLinkedTo(a, deal.Target) ? 1.0 : 0.5);
            }
            foreach (var a in deal.RequestArchitectures)
            {
                cost += GetArchitectureValue(a) * 1.5;
            }
            foreach (double value in new[] { GetTruceValue(deal), GetAllianceValue(deal) })
            {
                if (value > 0)
                {
                    gain += value;
                }
                else
                {
                    cost -= value;
                }
            }
        }

        /// <summary>
        /// 让利率：情面每30000让一成，最多让三成；情面为负则加价，最多加五成
        /// </summary>
        public static double GetConcessionRate(DiplomacyDeal deal)
        {
            return Math.Clamp(GetGoodwill(deal) / (GoodwillPerTenPercent * 10.0), -MaxPremium, MaxConcession);
        }

        /// <summary>
        /// 情面：双方关系、君主之间的好恶、使者的政治和与对方君主的亲疏
        /// </summary>
        public static int GetGoodwill(DiplomacyDeal deal)
        {
            Person leader = deal.Target.Leader;
            int value = 0;
            var relation = deal.GetRelation();
            if (relation != null)
            {
                value += Math.Clamp(relation.Relation, -500, 500) * 100;
            }
            if (leader.Hates(deal.Source.Leader))
            {
                value -= 30000;
            }
            else if (leader.IsCloseTo(deal.Source.Leader))
            {
                value += 20000;
            }
            value += GetEnvoyValue(deal.Envoys, leader);
            return value;
        }

        /// <summary>
        /// 随行的使者最多算几人，免得把满城武将都派去堆情面
        /// </summary>
        public const int MaxCountedCompanions = 2;

        /// <summary>
        /// 使者团的情面：估值最高的一位全算；随行的各按自己的估值算一半，只加不减，最多算两人；
        /// 有被对方君主厌恶的人同行，再减一笔
        /// </summary>
        public static int GetEnvoyValue(List<Person> envoys, Person leader)
        {
            if (envoys.Count == 0) return 0;

            var values = envoys.Select(x => GetSingleEnvoyValue(x, leader)).OrderByDescending(x => x).ToList();
            int companions = values.Skip(1).Take(MaxCountedCompanions).Sum(x => Math.Max(0, x) / 2);
            bool anyHated = envoys.Any(leader.Hates);
            return values[0] + companions - (anyHated ? 20000 : 0);
        }

        /// <summary>
        /// 一位使者的情面：政治、与对方君主的相性和亲疏
        /// </summary>
        public static int GetSingleEnvoyValue(Person envoy, Person leader)
        {
            int value = (envoy.Politics - 60) * 500 - Person.GetIdealOffset(envoy, leader) * 200;
            if (leader.Hates(envoy))
            {
                value -= 40000;
            }
            else if (leader.IsCloseTo(envoy))
            {
                value += 15000;
            }
            return value;
        }

        /// <summary>
        /// 城池的价值：规模、人口、开发度，重镇另加
        /// </summary>
        public static int GetArchitectureValue(Architecture a)
        {
            return 30000 * a.JianzhuGuimo + a.Population / 2 + (a.Agriculture + a.Commerce + a.Technology) * 30 + (a.IsImportant ? 100000 : 0);
        }

        /// <summary>
        /// 城池与这个势力的城相连，得到后好守好用
        /// </summary>
        public static bool IsLinkedTo(Architecture a, Faction faction)
        {
            foreach (var linked in a.AILandLinks)
            {
                if (linked.BelongedFaction == faction) return true;
            }
            foreach (var linked in a.AIWaterLinks)
            {
                if (linked.BelongedFaction == faction) return true;
            }
            return false;
        }

        // 缺钱的时候钱更值钱，钱多的时候没那么稀罕
        private static double GetFundFactor(Architecture a)
        {
            if (!a.IsFundEnough) return 1.3;
            return a.Fund > a.AbundantFund ? 0.8 : 1.0;
        }

        private static double GetFoodFactor(Architecture a)
        {
            return a.IsFoodEnough ? 1.0 : 1.3;
        }

        // 拿出去的东西比收进来的更舍不得
        private static double GetFundLossFactor(Architecture a)
        {
            return a.IsFundEnough ? 1.2 : 1.8;
        }

        private static double GetFoodLossFactor(Architecture a)
        {
            return a.IsFoodEnough ? 1.2 : 1.8;
        }

        /// <summary>
        /// 停战对接受方的利弊：比提议方弱就乐意停，比提议方强就等于放弃进攻的机会。不接壤的停战无关紧要
        /// </summary>
        private static double GetTruceValue(DiplomacyDeal deal)
        {
            if (deal.TruceDays <= 0) return 0;

            var relation = deal.GetRelation();
            int days = deal.TruceDays - (relation?.Truce ?? 0);
            if (days <= 0) return 0;

            double ratio = Math.Clamp((double)deal.Target.Power / Math.Max(1, deal.Source.Power), 0.2, 5);
            double perMonth = 20000 * (1 - ratio) - 10000;
            if (!deal.Source.adjacentTo(deal.Target))
            {
                perMonth *= 0.3;
            }
            return perMonth * days / 30.0;
        }

        /// <summary>
        /// 结盟对接受方的利弊：盟友越强越愿意，关系越好越愿意
        /// </summary>
        private static double GetAllianceValue(DiplomacyDeal deal)
        {
            if (!deal.Alliance) return 0;

            int relation = deal.GetRelation()?.Relation ?? 0;
            double ratio = Math.Clamp((double)deal.Source.Power / Math.Max(1, deal.Target.Power), 0, 2);
            return -80000 + (relation - AllyRelation) * 1000 + 60000 * ratio;
        }

        /// <summary>
        /// 只送不取，没有任何要对方付出的条款
        /// </summary>
        public static bool IsPureGift(DiplomacyDeal deal)
        {
            return deal.RequestFund == 0 && deal.RequestFood == 0 && deal.RequestArchitectures.Count == 0 && deal.TruceDays == 0 && !deal.Alliance && !deal.IsEmpty;
        }

        /// <summary>
        /// 条款交割后关系提升多少，由送出去的实利决定，与亲善相当（约500资金一点）
        /// </summary>
        public static int GetRelationGain(DiplomacyDeal deal)
        {
            // 对方得利关系上升，吃亏关系下降，各最多60。白给的援助不算吃亏，只折掉一点人情
            GetMaterialBalance(deal, out double gain, out double cost);
            double aid = Math.Min(cost, GetAidValue(deal));
            return Math.Clamp((int)((gain - cost + aid) / 500 - aid / AidValuePerRelation), -60, 60);
        }

        /// <summary>
        /// 对方态度分几档，给玩家一个大致印象，不透露准确的底线
        /// </summary>
        /// <returns>3欣然、2有所考虑、1颇为不满、0不屑一顾</returns>
        public static int GetAttitudeLevel(DiplomacyDeal deal)
        {
            if (IsPureGift(deal)) return 3;

            // 按得到的够付出的几成来分档，援助抵不掉的只取不予必定不屑一顾。接受线（够十成）落在“有所考虑”档内，不透露底线
            double gain = GetGainAndRequired(deal, out double required);
            if (required <= 0) return 3;
            double ratio = gain / required;
            if (ratio >= 1.2) return 3;
            if (ratio >= 0.8) return 2;
            if (ratio >= 0.3) return 1;
            return 0;
        }

        public static string GetAttitudeText(int level)
        {
            switch (level)
            {
                case 3:
                    return "欣然应允";
                case 2:
                    return "有所考虑";
                case 1:
                    return "颇为不满";
                default:
                    return "不屑一顾";
            }
        }

        /// <summary>
        /// 对方对条款的回应。条款本身不成立时直接拒绝并说明原因
        /// </summary>
        public static DiplomacyResponse Respond(DiplomacyDeal deal)
        {
            string invalid = deal.GetInvalidReason(false);
            if (invalid != null)
            {
                return new DiplomacyResponse { Kind = DiplomacyResponseKind.Reject, Text = invalid };
            }

            var relation = deal.GetRelation();
            if (relation.NegotiationCount >= MaxNegotiationsPerMonth)
            {
                return new DiplomacyResponse { Kind = DiplomacyResponseKind.Refuse, Text = "此事已议过多次，改日再谈吧。" };
            }

            if (IsPureGift(deal))
            {
                return new DiplomacyResponse { Kind = DiplomacyResponseKind.Accept, Text = "盛情难却，却之不恭。" };
            }

            int score = Evaluate(deal);
            if (score >= 0)
            {
                // 得到的抵不上付出，是靠援助才成的
                bool aided = GetMaterialValue(deal) < 0 && GetAidValue(deal) > 0;
                return new DiplomacyResponse { Kind = DiplomacyResponseKind.Accept, Text = aided ? "两家素来交好，些许钱粮，权当相助。" : "此议甚合我意，便依此缔约。" };
            }

            // 差多少就要求再加多少，优先要钱，钱不够再要粮
            int deficit = -score;
            Architecture source = deal.SourceArchitecture;
            Architecture capital = deal.Target.Capital;
            int extraFund = RoundUp(deficit / GetFundFactor(capital), 1000);
            if (source != null && extraFund <= source.Fund - deal.GiveFund)
            {
                return new DiplomacyResponse
                {
                    Kind = DiplomacyResponseKind.Counter,
                    ExtraFund = extraFund,
                    Text = "若再添资金 " + extraFund.ToString("N0") + "，此事可成。"
                };
            }
            int extraFood = RoundUp(deficit * FoodPerFund / GetFoodFactor(capital), 10000);
            if (source != null && extraFood <= source.Food - deal.GiveFood)
            {
                return new DiplomacyResponse
                {
                    Kind = DiplomacyResponseKind.Counter,
                    ExtraFood = extraFood,
                    Text = "若再添粮草 " + extraFood.ToString("N0") + "，此事可成。"
                };
            }
            return new DiplomacyResponse { Kind = DiplomacyResponseKind.Reject, Text = "条件相差太远，恕难从命。" };
        }

        /// <summary>
        /// 谈不成时记一次，开价太离谱还会伤关系
        /// </summary>
        public static void RecordFailure(DiplomacyDeal deal)
        {
            var relation = deal.GetRelation();
            if (relation == null) return;

            relation.NegotiationCount++;
            if (GetAttitudeLevel(deal) == 0)
            {
                relation.Worsen(5);
            }
        }

        private static int RoundUp(double value, int unit)
        {
            return (int)Math.Ceiling(value / unit) * unit;
        }
    }
}

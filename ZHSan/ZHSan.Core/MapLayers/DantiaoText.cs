using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using GameObjects;
using Newtonsoft.Json.Linq;
using Platforms;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    /// <summary>
    /// 单挑的台词、评书、兵器和善射的设定，从 Content\Data\Dantiao\DantiaoText.json 读取（MOD里有同名文件时用MOD的）。
    /// 每场单挑开始时重新读一次，改了文件不用重启游戏
    /// </summary>
    public static class DantiaoText
    {
        const string FilePath = @"Content\Data\Dantiao\DantiaoText.json";

        static Dictionary<string, List<string>> lines = new Dictionary<string, List<string>>();
        static List<KeyValuePair<string, List<string>>> weaponKinds = new List<KeyValuePair<string, List<string>>>();
        static Dictionary<string, string> personWeapons = new Dictionary<string, string>();
        static HashSet<string> archerPersons = new HashSet<string>();
        static List<string> archerKeywords = new List<string>();
        static readonly HashSet<string> warnedKeys = new HashSet<string>();

        public const string BowKind = "弓";
        public const string DefaultWeapon = "兵器";

        public static void Reload()
        {
            var newLines = new Dictionary<string, List<string>>();
            var newKinds = new List<KeyValuePair<string, List<string>>>();
            var newPersonWeapons = new Dictionary<string, string>();
            var newArcherPersons = new HashSet<string>();
            var newArcherKeywords = new List<string>();

            try
            {
                var json = JObject.Parse(Platform.Current.LoadText(Platform.Current.GetMODFile(FilePath)));

                if (json["兵器"] is JObject kinds)
                {
                    foreach (var p in kinds.Properties())
                    {
                        newKinds.Add(new KeyValuePair<string, List<string>>(p.Name, Strings(p.Value)));
                    }
                }
                if (json["人物兵器"] is JObject persons)
                {
                    foreach (var p in persons.Properties())
                    {
                        newPersonWeapons[p.Name] = p.Value.ToString();
                    }
                }
                if (json["善射"] is JObject archer)
                {
                    newArcherPersons.UnionWith(Strings(archer["人物"]));
                    newArcherKeywords.AddRange(Strings(archer["称号技能"]));
                }
                if (json["文本"] is JObject texts)
                {
                    foreach (var p in texts.Properties())
                    {
                        newLines[p.Name] = Strings(p.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                WebTools.TakeWarnMsg("读取单挑文本失败:" + FilePath, "DantiaoText.Reload", ex);
            }

            lines = newLines;
            weaponKinds = newKinds;
            personWeapons = newPersonWeapons;
            archerPersons = newArcherPersons;
            archerKeywords = newArcherKeywords;
            warnedKeys.Clear();
        }

        static List<string> Strings(JToken token)
        {
            if (token is JArray array) return array.Select(t => t.ToString()).ToList();
            if (token == null) return new List<string>();
            return new List<string>() { token.ToString() };
        }

        public static bool Has(string key) => lines.ContainsKey(key);

        /// <summary>
        /// 从一个或几个条目里合起来随机挑一句。vars是成对的占位符名和值，如 "攻", "关羽" 替换 {攻}
        /// </summary>
        public static string Pick(Random rng, string key, params object[] vars)
        {
            return PickAny(rng, new[] { key }, vars);
        }

        public static string PickAny(Random rng, IEnumerable<string> keys, params object[] vars)
        {
            var pool = new List<string>();
            foreach (var key in keys)
            {
                if (key != null && lines.TryGetValue(key, out var list))
                {
                    pool.AddRange(list);
                }
            }
            if (pool.Count == 0)
            {
                var name = String.Join("/", keys.Where(k => k != null));
                if (warnedKeys.Add(name))
                {
                    WebTools.TakeWarnMsg("单挑文本缺少条目:" + name, "DantiaoText.Pick", null);
                }
                return "";
            }
            return Fill(pool[rng.Next(pool.Count)], vars);
        }

        /// <summary>
        /// 按兵器挑：有“条目.兵器类别”时用它，否则用通用的条目。自动带上 {兵器}
        /// </summary>
        public static string PickFor(Random rng, string key, DantiaoGeneral owner, params object[] vars)
        {
            var all = vars.Concat(new object[] { "兵器", owner.Weapon }).ToArray();
            var specific = owner.WeaponKind == null ? null : key + "." + owner.WeaponKind;
            return Pick(rng, specific != null && Has(specific) ? specific : key, all);
        }

        static string Fill(string text, object[] vars)
        {
            for (int i = 0; i + 1 < vars.Length; i += 2)
            {
                text = text.Replace("{" + vars[i] + "}", vars[i + 1]?.ToString() ?? "");
            }
            return text;
        }

        /// <summary>
        /// 武将用的兵器：先看身上的兵器宝物（取最值钱的一件），没有再看“人物兵器”里的设定，都没有就是“兵器”。
        /// 返回兵器的叫法（如“刀”“矛”）和类别（如“刀”“枪”），弓不算近战兵器
        /// </summary>
        public static (string Weapon, string Kind) WeaponOf(Person person)
        {
            var best = person.Treasures
                .Select(t => (Treasure: t, Match: KindOf(t.Name ?? "")))
                .Where(x => x.Match != null && x.Match.Value.Kind != BowKind)
                .OrderByDescending(x => x.Treasure.Worth)
                .FirstOrDefault();
            if (best.Match != null)
            {
                return (best.Match.Value.Keyword, best.Match.Value.Kind);
            }
            if (personWeapons.TryGetValue(person.Name, out var named))
            {
                return (named, KindOf(named)?.Kind);
            }
            return (DefaultWeapon, null);
        }

        /// <summary>
        /// 按名字里的关键字认兵器类别，取最靠后的关键字，“三叉鬼头刀”认作刀
        /// </summary>
        static (string Kind, string Keyword)? KindOf(string name)
        {
            (string Kind, string Keyword)? found = null;
            var foundAt = -1;
            foreach (var kind in weaponKinds)
            {
                foreach (var keyword in kind.Value)
                {
                    var at = name.LastIndexOf(keyword, StringComparison.Ordinal);
                    if (at >= 0 && at + keyword.Length > foundAt)
                    {
                        foundAt = at + keyword.Length;
                        found = (kind.Key, keyword);
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// 善射：名单里有的，身上带弓的，或有射手、弩神之类称号技能的
        /// </summary>
        public static bool IsArcher(Person person)
        {
            if (archerPersons.Contains(person.Name)) return true;
            if (person.Treasures.Any(t => KindOf(t.Name ?? "")?.Kind == BowKind)) return true;

            var names = person.Skills.Values.Select(s => s.Name).Concat(person.Titles.Select(t => t.Name));
            return names.Any(n => n != null && archerKeywords.Any(k => n.Contains(k)));
        }
    }
}

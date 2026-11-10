using GameManager;
using GameObjects;
using GameObjects.FactionDetail;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 外交关系列表：列出一个势力与其他势力的关系和剩余停战天数，样式与信息列表一致。
    /// 可以按友好、中立、敌对、停战筛选，点表头排序；点一行在右侧显示对方势力的详情
    /// </summary>
    internal static class DiplomaticRelationListDialog
    {
        /// <summary>
        /// 显示外交关系列表
        /// </summary>
        public static void Show(Faction faction)
        {
            // 默认关系好的排在前面，与外交面板一致；关系相同时停战久的在前
            var displays = Session.Current.Scenario.GetDiplomaticRelationDisplays(faction.ID)
                .OrderByDescending(x => x.Relation)
                .ThenByDescending(x => x.Truce)
                .ToList();
            var columns = new[]
            {
                new InfoColumn<DiplomaticRelationDisplay>("势力", 0, (x, width) => InformationLists.CreateDiplomacyFactionCell(GetOtherFaction(faction, x), x.FactionName), x => x.FactionName)
                {
                    MinWidth = 160
                },
                new InfoColumn<DiplomaticRelationDisplay>("关系", 80, x => x.Relation.ToString(), x => x.Relation,
                    x => DiplomacyDialog.GetStatusColor(x.LinkedDiplomaticRelation)),
                new InfoColumn<DiplomaticRelationDisplay>("停战", 80, x => x.Truce > 0 ? x.Truce + "天" : "——", x => x.Truce,
                    x => x.Truce > 0 ? MyraSkin.TextHighlight : MyraSkin.TextDisabled)
            };
            var filters = new List<InfoFilter<DiplomaticRelationDisplay>>
            {
                new InfoFilter<DiplomaticRelationDisplay>("友好", x => x.Relation > 0),
                new InfoFilter<DiplomaticRelationDisplay>("中立", x => x.Relation == 0),
                new InfoFilter<DiplomaticRelationDisplay>("敌对", x => x.Relation < 0),
                new InfoFilter<DiplomaticRelationDisplay>("停战中", x => x.Truce > 0)
            }.Where(f => displays.Any(f.Match));
            InfoListDialog<DiplomaticRelationDisplay>.ShowWithDetail(faction.Name + " 外交关系", displays, columns, filters, (panel, x, width) =>
            {
                Faction other = GetOtherFaction(faction, x);
                if (other != null)
                {
                    InformationLists.FillFaction(panel, other, width);
                }
                else
                {
                    panel.Widgets.Add(MyraWidgets.CreateLabel(x.FactionName + " 已灭亡", 16, MyraSkin.TextSecondary));
                }
            });
        }

        // 关系的另一方，已灭亡的势力取不到，返回null
        private static Faction GetOtherFaction(Faction faction, DiplomaticRelationDisplay display)
        {
            DiplomaticRelation relation = display.LinkedDiplomaticRelation;
            return relation.RelationFaction1ID == faction.ID ? relation.RelationFaction2 : relation.RelationFaction1;
        }
    }
}

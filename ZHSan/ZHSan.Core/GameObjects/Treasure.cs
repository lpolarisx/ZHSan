using GameDatas;
using GameManager;
using GameObjects.Influences;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects;

/// <summary>
/// 宝物
/// </summary>
[DataContract]
public class Treasure : GameObject
{
    /// <summary>
    /// 图像
    /// </summary>
    [DataMember]
    public int Pic { get; set; }

    /// <summary>
    /// 价值
    /// </summary>
    [DataMember]
    public int Worth { get; set; }

    /// <summary>
    /// 已出现
    /// </summary>
    [DataMember]
    public bool Available { get; set; }

    /// <summary>
    /// 隐藏于建筑
    /// </summary>
    [DataMember]
    public int HidePlaceIDString { get; set; }

    /// <summary>
    /// 宝物种类：此值相同的话，这些宝物不叠加
    /// </summary>
    [DataMember]
    public int TreasureGroup { get; set; }

    /// <summary>
    /// 出现年
    /// </summary>
    [DataMember]
    public int AppearYear { get; set; }

    /// <summary>
    /// 属于人物
    /// </summary>
    [DataMember]
    public int BelongedPersonIDString { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    [DataMember]
    public string InfluencesString { get; set; }

    /// <summary>
    /// 介绍
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// 耐久
    /// </summary>
    [DataMember]
    public int Durability { get; set; }

    public Treasure() {}

    public Treasure(TreasureConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Pic = config.Pic;
        Worth = config.Worth;
        Available = config.Available;
        HidePlaceIDString = config.HidePlaceIDString;
        TreasureGroup = config.TreasureGroup;
        AppearYear = config.AppearYear;
        BelongedPersonIDString = config.BelongedPersonIDString;
        InfluencesString = config.InfluencesString;
        Description = config.Description;
        Durability = config.Durability;
    }

    public TreasureConfig ToConfig()
    {
        return new TreasureConfig
        {
            Id = ID,
            Name = Name,
            Pic = Pic,
            Worth = Worth,
            Available = Available,
            HidePlaceIDString = HidePlaceIDString,
            TreasureGroup = TreasureGroup,
            AppearYear = AppearYear,
            BelongedPersonIDString = BelongedPersonIDString,
            InfluencesString = InfluencesString,
            Description = Description,
            Durability = Durability,
        };
    }

    public Person BelongedPerson;

    public Architecture HidePlace;

    public Dictionary<int, Influence> Influences { get; set; } = new();

    public string BelongedPersonString => BelongedPerson?.Name ?? "----";

    public string HidePlaceString => HidePlace?.Name ?? "----";

    public string InfluenceString => string.Join("•", Influences.Values.Select(x => x.Description));

    private PlatformTexture picture;

    public PlatformTexture Picture
    {
        get
        {
            if (picture == null)
            {
                try
                {
                    picture = CacheManager.GetTempTexture("Content/Textures/Resources/Treasure/" + Pic.ToString() + ".jpg");
                }
                catch
                {
                    picture = null;
                }
            }

            return picture;
        }
    }

    //public void disposeTexture()
    //{
    //    if (this.picture != null)
    //    {
    //        this.picture.Dispose();
    //        this.picture = null;
    //    }
    //}
}
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.MapDetail;

[DataContract]
public class TerrainDetailTable
{
    [DataMember]
    public Dictionary<int, TerrainDetail> TerrainDetails = new();

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="terrainDetail"></param>
    /// <returns></returns>
    public bool Add(TerrainDetail terrainDetail)
    {
        return TerrainDetails.TryAdd(terrainDetail.ID, terrainDetail);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Remove(int id)
    {
        return TerrainDetails.Remove(id);
    }

    /// <summary>
    /// 查找
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TerrainDetail Get(int id)
    {
        TerrainDetails.TryGetValue(id, out var terrainDetail);
        return terrainDetail;
    }

    public int Count => TerrainDetails.Count;

    public void Clear() => TerrainDetails.Clear();

    public GameObjectList GetTerrainDetailList() => [.. TerrainDetails.Values];
}

using Microsoft.Xna.Framework;

namespace GameDatas;

public class MapConfig
{
    public Point JumpPosition { get; set; }

    public int TileWidthMin { get; set; }

    public int TileWidthMax { get; set; }

    public string MapName { get; set; }

    public string MapDataString { get; set; }

    public Point MapDimensions { get; set; }

    public int TileHeight { get; set; }

    public int TileWidth { get; set; }

    public int NumberOfTiles { get; set; }

    public int NumberOfSquaresInEachTile { get; set; }

    public bool UseSimpleArchImages { get; set; }
}
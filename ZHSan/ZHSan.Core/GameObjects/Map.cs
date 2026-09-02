using GameDatas;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace GameObjects
{
    [DataContract]
    public class Map
    {
        [DataMember]
        public Point JumpPosition { get; set; }

        [DataMember]
        public int TileWidthMin { get; set; } = 30;

        [DataMember]
        public int TileWidthMax { get; set; } = 100;

        private string dituwenjian;

        [DataMember]
        public string MapName
        {
            get
            {
                if (string.IsNullOrEmpty(dituwenjian))
                {
                    return null;
                } 
                else 
                {
                    return dituwenjian;
                }
            }
            set
            {
                if (value.EndsWith(".jpg"))
                {
                    value = value.Substring(0, value.Length - 4);
                }
                
                dituwenjian = value;
            }
        }

        public void Init()
        {
            if (TileWidthMin == 0)
            {
                TileWidthMin = 30;
            }
            if (TileWidthMax == 0)
            {
                TileWidthMax = 100;
            }
        }

        public void Clear()
        {
            MapDimensions = Point.Zero;
            MapData = null;
        }

        public bool LoadMapData(string[] mapDataValueString, int X, int Y)
        {
            MapDimensions = new Point(X, Y);

            if (mapDataValueString.Length != this.MapTileCount)
            {
                throw new Exception("The map data count does not match the MapTileCount");
            }

            MapData = new int[X, Y];
            for (int i = 0; i < this.MapTileCount; i++)
            {
                try
                {
                    MapData[i % X, i / X] = int.Parse(mapDataValueString[i]);
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.ToString());
                }
            }
            return true;
        }

        public bool LoadMapData(string mapdata, int X, int Y)
        {
            MapDimensions = new Point(X, Y);

            char[] separator = new char[] { ' ', '\n', '\r', '\t' };
            string[] strArray = mapdata.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != this.MapTileCount)
            {
                throw new Exception("The map data count does not match the MapTileCount");
            }

            MapData = new int[X, Y];
            for (int i = 0; i < this.MapTileCount; i++)
            {
                try
                {
                    MapData[i % X, i / X] = int.Parse(strArray[i]);
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.ToString());
                }
            }
            return true;
        }

        public bool LoadMapDataFromDataBase(string connectionString)
        {
            return true;
        }

        public bool PositionOutOfRange(Point mapPosition)
        {
            return mapPosition.X < 0 || mapPosition.Y < 0 || mapPosition.X >= MapDimensions.X || mapPosition.Y >= MapDimensions.Y;
        }

        public void Replace(int terrainID1, int terrainID2)
        {
            for (int i = 0; i < MapDimensions.Y; i++)
            {
                for (int j = 0; j < MapDimensions.X; j++)
                {
                    if (MapData[j, i] == terrainID1)
                    {
                        MapData[j, i] = terrainID2;
                    }
                }
            }
        }

        public string SaveToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < MapDimensions.Y; i++)
            {
                for (int j = 0; j < MapDimensions.X; j++)
                {
                    builder.Append(MapData[j, i].ToString() + " ");
                }
            }
            return builder.ToString();
        }
        
        public int[,] MapData { get; set; }

        [DataMember]
        public string MapDataString { get; set; }

        [DataMember]
        public Point MapDimensions { get; set; }

        public int MapTileCount => MapDimensions.X * MapDimensions.Y;

        [DataMember]
        public int TileHeight { get; set; } = 100;
        
        [DataMember]
        public int TileWidth { get; set; } = 100;

        public int TotalTileHeight => TileHeight * MapDimensions.Y;

        public int TotalTileWidth => TileWidth * MapDimensions.X;

        [DataMember]
        public int NumberOfTiles { get; set; } = 20;

        [DataMember]
        public int NumberOfSquaresInEachTile { get; set; } = 10;

        [DataMember]
        public bool UseSimpleArchImages { get; set; }

        public Map() {}

        public Map(MapConfig config)
        {
            JumpPosition = config.JumpPosition;
            TileWidthMin = config.TileWidthMin;
            TileWidthMax = config.TileWidthMax;
            MapName = config.MapName;
            MapDataString = config.MapDataString;
            MapDimensions = config.MapDimensions;
            TileHeight = config.TileHeight;
            TileWidth = config.TileWidth;
            NumberOfTiles = config.NumberOfTiles;
            NumberOfSquaresInEachTile = config.NumberOfSquaresInEachTile;
            UseSimpleArchImages = config.UseSimpleArchImages;
        }

        public MapConfig ToConfig()
        {
            return new MapConfig
            {
                JumpPosition = JumpPosition,
                TileWidthMin = TileWidthMin,
                TileWidthMax = TileWidthMax,
                MapName = MapName,
                MapDataString = MapDataString,
                MapDimensions = MapDimensions,
                TileHeight = TileHeight,
                TileWidth = TileWidth,
                NumberOfTiles = NumberOfTiles,
                NumberOfSquaresInEachTile = NumberOfSquaresInEachTile,
                UseSimpleArchImages = UseSimpleArchImages,
            };
        }
    }
}
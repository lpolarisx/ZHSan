using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameManager;

namespace WorldOfTheThreeKingdoms.GameScreens.ScreenLayers
{
    public class MapTile:Tile 
    {
        public string  number;


#pragma warning disable CS0108 // 'MapTile.TileTexture' hides inherited member 'Tile.TileTexture'. Use the new keyword if hiding was intended.
        public Texture2D TileTexture;
#pragma warning restore CS0108 // 'MapTile.TileTexture' hides inherited member 'Tile.TileTexture'. Use the new keyword if hiding was intended.

        //淡入进度，0为刚加载完（仍显示默认地图块），1为完全显示
        public float FadeAlpha = 1f;

         
    }



}

using GameDatas;
using GameManager;
using Microsoft.Xna.Framework;

namespace GameObjects.Animations;

public class Animation : GameObject
{
    public string TextureFileName { get; set; }

    public int TextureWidth { get; set; }

    public int TextureHeight { get; set; }

    public bool Back { get; set; }

    public int FrameCount { get; set; }

    public int StayCount { get; set; }

    public Animation(AnimationConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        TextureFileName = config.TextureFileName;
        TextureWidth = config.TextureWidth;
        TextureHeight = config.TextureHeight;
        Back = config.Back;
        FrameCount = config.FrameCount;
        StayCount = config.StayCount;
    }

    public Rectangle GetCurrentDisplayRectangle(ref int frameIndex, ref int stayIndex, int width, int row, out bool EndLoop, bool hold)
    {
        EndLoop = false;
        if (!hold)
        {
            stayIndex++;
            if (stayIndex >= this.StayCount * Setting.Current.GlobalVariables.FastBattleSpeed / 4)
            {
                stayIndex = 0;
                frameIndex++;
                if (frameIndex >= (this.FrameCount - 1))
                {
                    EndLoop = true;
                }
            }
        }
        return new Rectangle(width * frameIndex, width * row, width, width);
    }

    //public void disposeTexture()
    //{
    //    if (this.texture != null)
    //    {
    //        this.texture.Dispose();
    //        this.texture = null;
    //    }
    //}

    private PlatformTexture texture;

    public PlatformTexture Texture
    {
        get
        {
            if (this.texture == null)
            {
                //try
                //{
                this.texture = CacheManager.GetTempTexture(this.TextureFileName);
                this.texture.Width = this.TextureWidth;
                this.texture.Height = this.TextureHeight;
                //}
                //catch (OutOfMemoryException)
                //{
                //    this.texture = new Texture2D(1, 1);
                //}
            }
            return this.texture;
        }
    }
}
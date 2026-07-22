using GameEnums;
using GameGlobal;
using System;
using System.Runtime.InteropServices;


namespace GameObjects
{

    [StructLayout(LayoutKind.Sequential)]
    public struct InformationTile
    {
        private int lowCount;
        private int middleCount;
        private int highCount;
        private int fullCount;
        public InformationLevel Level
        {
            get
            {
                if (this.fullCount > 0)
                {
                    return InformationLevel.Full;
                }
                if (this.highCount > 0)
                {
                    return InformationLevel.High;
                }
                if (this.middleCount > 0)
                {
                    return InformationLevel.Medium;
                }
                if (this.lowCount > 0)
                {
                    return InformationLevel.Low;
                }
                return InformationLevel.None;
            }
        }
        public void AddInformationLevel(InformationLevel level)
        {
            switch (level)
            {
                case InformationLevel.Low:
                    this.lowCount++;
                    break;

                case InformationLevel.Medium:
                    this.middleCount++;
                    break;

                case InformationLevel.High:
                    this.highCount++;
                    break;

                case InformationLevel.Full:
                    this.fullCount++;
                    break;
            }
        }

        public void RemoveInformationLevel(InformationLevel level)
        {
            switch (level)
            {
                case InformationLevel.Low:
                    this.lowCount--;
                    break;

                case InformationLevel.Medium:
                    this.middleCount--;
                    break;

                case InformationLevel.High:
                    this.highCount--;
                    break;

                case InformationLevel.Full:
                    this.fullCount--;
                    break;
            }
        }

        public static string InformationString(InformationLevel level)
        {
            return level.ToString();
        }

        public override string ToString()
        {
            return InformationString(this.Level);
        }
    }
}


using GameDatas;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class NoFoodPosition
    {
        [DataMember]
        public int Days { get; set; }

        [DataMember]
        public Point Position { get; set; }

        public NoFoodPosition(Point position, int days)
        {
            Position = position;
            Days = days;
        }

        public NoFoodPosition(NoFoodConfig config)
        {
            Position = config.Position;
            Days = config.Days;
        }

        public NoFoodConfig ToConfig()
        {
            return new NoFoodConfig
            {
                Position = Position,
                Days = Days,
            };
        } 
    }
}
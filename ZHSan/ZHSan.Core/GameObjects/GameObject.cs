using GameGlobal;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    [KnownType(typeof(AttackTargetKind))]
    [KnownType(typeof(CastDefaultKind))]
    [KnownType(typeof(CastTargetKind))]
    [KnownType(typeof(Captive))]
    [KnownType(typeof(CharacterKind))]
    [KnownType(typeof(Event))]
    [KnownType(typeof(Person))]
    [KnownType(typeof(Architecture))]
    [KnownType(typeof(Legion))]
    [KnownType(typeof(Military))]
    [KnownType(typeof(Routeway))]
    [KnownType(typeof(Treasure))]
    [KnownType(typeof(TroopEvent))]
    [KnownType(typeof(Troop))]
    [KnownType(typeof(Faction))]
    [KnownType(typeof(YearTableEntry))]
    
    public class GameObject
    {
        public float Scale;

        public static bool GetChance(int chance)
        {
            if (chance <= 0) return false;

            if (chance >= 100) return true;

            if (StaticMethods.Random(100) < chance) return true;

            return false;
        }

        public static bool GetChance(int chance, int root)
        {
            if (chance <= 0)
            {
                return false;
            }
            return ((chance >= root) || (Random(root) < chance));
        }

        public GameObjectList GetGameObjectList()
        {
            GameObjectList list = new GameObjectList();
            list.Add(this);
            return list;
        }

        public static int Random(int maxValue)
        {
            return StaticMethods.Random(maxValue);
        }

        public static int Random(int min, int max)
        {
            return StaticMethods.Random(Math.Abs(max - min) + 1) + Math.Min(max, min);
        }

        public static int RandomGaussian(double mean, double var)
        {
            double u1 = StaticMethods.Random();
            double u2 = StaticMethods.Random();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            return (int) Math.Round(mean + (var / 3) * randStdNormal);
        }


        public static int RandomGaussianRange(int lo, int hi)
        {
            return RandomGaussian((hi + lo) / 2.0, Math.Abs(hi - lo) / 2.0);
        }

        public static int Square(int num)
        {
            return (num * num);
        }

        public override string ToString()
        {
            return this.Name;
        }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        public bool Selected { get; set; }

        public string TextDestinationString { get; set; }

        public string TextResultString { get; set; }

        public static T WeightedRandom<T>(Dictionary<T, float> weights)
        {
            if (weights.Count == 1)
            {
                var enu = weights.Keys.GetEnumerator();
                enu.MoveNext();
                return enu.Current;
                //return weights.GetEnumerator().Current.Key;
            }

            int randMax = 10000;
            double sum = 0;
            foreach (double i in weights.Values)
            {
                sum += i;
            }

            if (sum <= 0)
            {
                var enu = weights.Keys.GetEnumerator();
                enu.MoveNext();
                return enu.Current;
            }

            int p = GameObject.Random(randMax);
            double pt = 0;
            foreach (KeyValuePair<T, float> td in weights)
            {
                pt += td.Value / sum * randMax;
                if (p < pt)
                {
                    return td.Key;
                }
            }
            throw new InvalidOperationException("null resulted in WeightedRandom");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameObject)) return false;
            return ID == ((GameObject)obj).ID;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
using GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class FactionListWithQueue
    {
        private Queue<Faction> factionQueue = new Queue<Faction>();

        public Faction RunningFaction;

        [DataMember]
        public string FactionQueue { get; set; }

        public void BuildQueue(bool preUserControlFinished)
        {
            factionQueue = new Queue<Faction>();

            var factions = Session.Current.Scenario.Factions.Values.ToList();
            factions.Sort((a, b) => a.Power.CompareTo(b.Power));

            foreach (var faction in factions)
            {
                SetFactionInQueue(faction, preUserControlFinished);
            }
        }

        public bool HasFactionInQueue(IEnumerable<Faction> factions)
        {
            foreach (var faction in factions)
            {
                if (IsFactionInQueue(faction))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFactionInQueue(Faction faction)
        {
            foreach (Faction faction2 in factionQueue)
            {
                if (faction2 == faction)
                {
                    return true;
                }
            }
            return false;
        }

        public void RunQueue()
        {
            if (RunningFaction != null)
            {
                if (RunningFaction.Run())
                {
                    RunningFaction = null;
                }
            }
            else if (!QueueEmpty)
            {
                RunningFaction = factionQueue.Dequeue();
                if (this.RunningFaction != null)
                {
                    if (this.RunningFaction.Leader.BelongedFaction == null)
                    {
                        this.RunningFaction = null;
                    }
                    else
                    {
                        Session.Current.Scenario.CurrentFaction = this.RunningFaction;
                        if (this.RunningFaction.Run())
                        {
                            this.RunningFaction = null;
                        }
                    }
                }
            }
        }

        public string SaveQueueToString()
        {
            string str = "";
            if (this.factionQueue != null)
            { 
                foreach (Faction faction in this.factionQueue)
                {
                    str = str + " " + faction.ID.ToString();
                }
            }
            return str;
        }

        public void SetControlling(bool controlling)
        {
            var factions = Session.Current.Scenario.Factions.Values;

            foreach (var faction in factions)
            {
                faction.Controlling = controlling;
            }
        }

        private void SetFactionInQueue(Faction faction, bool preUserControlFinished)
        {
            factionQueue.Enqueue(faction);
            faction.Passed = false;
            faction.PreUserControlFinished = preUserControlFinished;
            faction.AIFinished = false;
        }

        public bool QueueEmpty => factionQueue.Count == 0;
    }
}
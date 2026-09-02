using GameEnums;
using GameGlobal;
using GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class TroopListWithQueue
    {
        public List<Troop> AmbushList { get; set; } = new();

        public Queue<Troop> CurrentQueue = new();
        public Troop CurrentTroop;
        private bool queueEnded = true;
        private Queue<Troop> troopQueue = new();

        public void Init()
        {
            queueEnded = true;
            AmbushList = new();
            CurrentQueue = new();
            troopQueue = new();
        }

        public void BuildQueue()
        {
            queueEnded = false;
            if (troopQueue.Count != 0)
            {
                throw new Exception("troopQueue is not empty before building");
            }

            var troops = Session.Current.Scenario.Troops.Values.ToList();

            AmbushList.Clear();

            if (Session.GlobalVariables.MilitaryKindSpeedValid)
            {
                troops.Sort((a, b) => b.Speed.CompareTo(a.Speed));
            }
            else
            {
                troops = StaticMethods.GetRandomList(troops);
            }

            foreach (var troop in troops)
            {
                if (troop.CanMoveAnyway())
                {
                    troop.InitializeInQueue();
                    if (troop.Status == TroopStatus.Ambushing)
                    {
                        AmbushList.Add(troop);
                    }
                    else
                    {
                        troopQueue.Enqueue(troop);
                    }
                }
                troop.Operated = false;
                troop.SelectedMove = false;
                troop.SelectedAttack = false;
                troop.Controllable = true;
            }
        }

        private bool CheckAmbushList()
        {
            Troop targetTroop = null;

            foreach (var troop in AmbushList)
            {
                if (troop.ToDoCombatAction())
                {
                    troop.DoCombatAction();
                    if (troop.OperationDone)
                    {
                        targetTroop = troop;
                        break;
                    }
                }
            }

            AmbushList.Remove(targetTroop);

            return targetTroop != null;
        }

        public void Clear()
        {
            troopQueue.Clear();
            CurrentQueue.Clear();
            AmbushList.Clear();
        }

        public void CurrentQueueTroopMove()
        {
            
            if (CurrentTroop != null)
            {

                TroopMoveThread(CurrentTroop);
                /*Thread thread;
                
                thread = new Thread(new ThreadStart(this.CurrentTroop.Move));
                thread.Start();
                thread.Join();*/
                
                if (CurrentTroop.StepNotFinished || CurrentTroop.MovabilityLeft <= 0)
                {
                    if (!CurrentTroop.OperationDone)
                    {
                        troopQueue.Enqueue(CurrentTroop);
                    }
                    CurrentTroop = null;
                }
            }
            else if (!CheckAmbushList())
            {
                var queue = new Queue<Troop>();

                if (CurrentQueue.Count == 0 && troopQueue.Count > 0)
                {
                    CurrentQueue.Enqueue(troopQueue.Dequeue());
                }

                while (CurrentQueue.Count > 0)
                {
                    Troop item = CurrentQueue.Dequeue();
                    if (!item.Destroyed)
                    {
                        if (item.Destroyed || 
                            (item.Status != TroopStatus.Normal && item.Status != TroopStatus.Rumour && item.Status != TroopStatus.Attract))
                        {
                            item.MovabilityLeft = -1;
                            item.OperationDone = true;
                        }

                        if (!item.Destroyed)
                        {
                            if (item.MovabilityLeft > 0)
                            {
                                TroopChangeRealDestination(item);
                                TroopMoveThread(item);
                            }

                            if (item.MovabilityLeft <= 0)
                            {
                                if (!item.HasToDoCombatAction && item.ToDoCombatAction())
                                {
                                    item.HasToDoCombatAction = true;
                                    CurrentQueue.Enqueue(item);
                                    break;
                                }
                                if (item.HasToDoCombatAction)
                                {
                                    item.HasToDoCombatAction = false;
                                    item.DoCombatAction();
                                    CurrentQueue.Enqueue(item);
                                    break;
                                }
                            }
                        }

                        if (item.Destroyed ||
                            (item.Status != TroopStatus.Normal && item.Status != TroopStatus.Rumour && item.Status != TroopStatus.Attract))
                        {
                            item.MovabilityLeft = -1;
                            item.OperationDone = true;
                        }
                       
                        if (!item.OperationDone && item.OffenceOnlyBeforeMove && item.Position != item.PreviousPosition)
                        {
                            item.OperationDone = true;
                        }
                        if ((!item.StepNotFinished || item.chongshemubiaoweizhibiaoji) && item.MovabilityLeft >= 0)
                        {
                            CurrentTroop = item;
                            break;
                        }

                        if (!queueEnded)
                        {
                            if (item.MovabilityLeft > 0)
                            {
                                item.WaitOnce = true;
                                queue.Enqueue(item);
                            }
                            else if (!(item.OperationDone || item.QueueEnded))
                            {
                                queue.Enqueue(item);
                            }
                        }
                    }
                }

                while (queue.Count > 0)
                {
                    troopQueue.Enqueue(queue.Dequeue());
                }
                if (!queueEnded && TotallyEmpty)
                {
                    queueEnded = true;

                    var list = new List<Troop>();
                    foreach (var troop in Session.Current.Scenario.Troops.Values)
                    {
                        if (troop.QueueEnded)
                        {
                            list.Add(troop);
                        }
                    }

                    foreach (var troop in StaticMethods.GetRandomList(list))
                    {
                        troopQueue.Enqueue(troop);
                    }
                }
            }
        }

        private void TroopChangeRealDestination(Troop troop)
        {
            if (troop.mingling == "Attack" || troop.mingling == "Stratagem")
            {
                if(troop.CurrentStratagem != null) { troop.mingling = "Stratagem"; }
                if (troop.TargetTroop != null)
                {
                    if (!((troop.mingling == "Attack" && troop.CanAttack(troop.TargetTroop)) ||
                    (troop.CurrentStratagem != null  && troop.CanStratagem(troop.TargetTroop))))
                    {
                        troop.RealDestination = troop.TargetTroop.Position;
                    } else
                    {
                        troop.RealDestination = troop.Position;
                    }
                }
                else if (troop.TargetArchitecture != null && troop.mingling == "Attack" && !troop.CanAttack(troop.TargetArchitecture))
                {
                    troop.RealDestination = troop.TargetArchitecture.Position;
                }

                else if(troop.TargetArchitecture != null && troop.TargetArchitecture.Endurance <= 0)
                { 
                    troop.RealDestination = troop.TargetArchitecture.Position;
                }
                else troop.RealDestination = troop.Position;
            }
            else if (troop.TargetTroop != null && troop.Will.ToString() == "行军" && troop.mingling!= "Move" )//&& 
                //((troop.CurrentStratagem==null && troop.CanAttack(troop.TargetTroop))
                //|| (troop.CurrentStratagem != null && troop.CanStratagem(troop.TargetTroop))))//ai//取消见面能打就站桩
            {
                if (troop.BaseAttackEveryAround || troop.AttackEveryAround)
                {//修复雷霆战法攻击方式
                    foreach (Troop troop2 in troop.GetAllOtherTroopsInView())
                    {
                        if ((!troop.AttackedTroopList.HasGameObject(troop2)) && troop.CanAttack(troop2) && (!troop.TroopNoAccidentalInjury || !troop.IsFriendly(troop2.BelongedFaction)))
                        {
                            troop.AttackTroop(troop2);
                        }
                    }
                }
                //troop.RealDestination = troop.Position;
            }
            else if (troop.TargetArchitecture != null)
            {
                //troop.RealDestination = troop.TargetArchitecture.Position;
            }
        }

        private void TroopMoveThread(Troop troop)
        {
            troop.Move();
            
            /*Thread thread;

            thread = new Thread(new ThreadStart(troop.Move));
            thread.Start();
            thread.Join();

            thread = null;*/
        }

        public void FinalizeQueue()
        {
            foreach (var troop in Session.Current.Scenario.Troops.Values)
            {
                troop.FinalizeInQueue();
            }
        }

        public void MoveTroopsFromQueueToCurrentQueue(int n)
        {
            n = Math.Min(n, troopQueue.Count);

            for (int i = 0; i < n; i++)
            {
                CurrentQueue.Enqueue(troopQueue.Dequeue());
            }
        }

        public void StepAnimationIndex(int steps)
        {
            if (CurrentTroop != null)
            {
                CurrentTroop.AddMoveAnimationIndex(steps);
            }

            foreach (var troop in CurrentQueue)
            {
                if (troop.Action != TroopAction.Stop)
                {
                    troop.AddMoveAnimationIndex(steps);
                }
            }
        }

        public bool CurrentQueueEmpty => CurrentQueue.Count == 0 && CurrentTroop == null;

        public bool QueueEmpty => troopQueue.Count == 0;

        public bool TotallyEmpty => QueueEmpty && CurrentQueueEmpty;
    }
}
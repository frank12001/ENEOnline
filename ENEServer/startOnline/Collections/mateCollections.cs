using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace startOnline
{
    /// <summary>
    /// 配對的主要"集合"存放的class
    /// </summary>
    public class mateCollections
    {
        public mateCollections()
        {
            mainQueue = new List<Baseplayer>();
            TeamQueue2 = new List<Grouplayer>();
            TeamQueue3 = new List<Grouplayer>();
            TeamQueue4 = new List<Grouplayer>();
            TeamQueue5 = new List<Grouplayer>();
            #region 醫院程式碼
            subQueue = new Queue<Baseplayer[]>();
            waitingtobeD = new List<int>();
            #endregion
        }
        List<Baseplayer> mainQueue;
        List<Grouplayer> TeamQueue2;
        List<Grouplayer> TeamQueue3;
        List<Grouplayer> TeamQueue4;
        List<Grouplayer> TeamQueue5;
        #region 醫院程式碼
        Queue<Baseplayer[]> subQueue; //for Group
        List<int> waitingtobeD; //等待被刪除，算是緩衝的一條線
        #endregion

        #region mainQueue 相關功能
        /// <summary>
        /// 回傳null 的話，就是進入排隊。回傳BaseRoom的話，就是有10個人了，準備加入gamingCollection
        /// </summary>
        /// <param name="me">自己</param>
        /// <returns></returns>
        public BaseRoom QueueorGaming(Baseplayer me)
        {
            lock ("mainQueueLock")
            {
                #region 醫院程式碼
                //在這邊寫對SubQueue的檢查 ，可以的話先加入mainQueue再把me排進去 ，要注意me一定要被排進去，所以在檢查能不能被排時時，要用總人數9去算
                if (subQueue.Count > 0 )
                {
                    Baseplayer[] markremove = subQueue.Peek();
                    //容量夠不夠
                    if (mainQueue.Count + subQueue.Peek().Length <= 9 && markremove[0] != null) //因為還要排me所以只能到9
                    {
                        //如果加入後會變成不同隊
                        if (mainQueue.Count < 5 && mainQueue.Count + subQueue.Peek().Length > 5)
                        {
                            //不要加入
                        }
                        else if (markremove[0]==null)
                        {
                            //直接移除
                            subQueue.Dequeue();
                        }
                        else
                        {
                            //將subQueue的第一個加入mainQueue
                            Baseplayer[] players = subQueue.Dequeue();
                            for (int i = 0; i < players.Length; i++)
                            {
                                if (!waitingtobeD.Contains(players[i].uid))
                                    mainQueue.Add(players[i]);
                                else
                                    waitingtobeD.Remove(players[i].uid);
                            }
                        }

                    }
                }
                #endregion
                if (mainQueue.Count == 9) 
                {
                    BaseRoom Room = new BaseRoom(mainQueue[0], mainQueue[1], mainQueue[2], mainQueue[3], mainQueue[4], mainQueue[5], mainQueue[6], mainQueue[7], mainQueue[8],me);
                    mainQueue.Clear();
                    return Room;
                }
                else //if (mainQueue.Count < 9)
                {
                    mainQueue.Add(me);
                    //foreach(Baseplayer p in mainQueue)
                    //{
                   //     MessageBox.Show("In mainQueue " + p.uid.ToString() + "//" + p.name);
                   // }
                    return null;
                }
            }
        }
        /// <summary>
        /// 取消排隊。
        /// </summary>
        /// <param name="uid">要取消人的uid</param>
        public void CancelQ(int uid)
        {
            lock ("mainQueueLock")
            {
                foreach (Baseplayer player in mainQueue)
                {
                    if (player.uid.Equals(uid))
                    {
                        mainQueue.Remove(player);
                        break;
                    }

                }
            }
        }
        #endregion
        #region 醫院程式碼
        /// <summary>
        /// 利用uid去查有沒有在mainQueue裡
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool MainQueueContain(int uid)
        {
            lock ("mainQueueLock")
            {
                foreach (Baseplayer player in mainQueue)
                {
                    if (player.uid == uid)
                        return true;
                }
                return false;
            }
        }
        #endregion
        #region 醫院程式碼 subQueue 相關功能
        public void QueueorGaming(Baseplayer[] myGroup)
        {
            lock ("mainQueueLock")
            {
                if (mainQueue.Count == 0) //當現在mainQueue沒有人時，直接加入
                {
                    for (int i = 0; i < myGroup.Length; i++)
                    {
                        QueueorGaming(myGroup[i]);
                    }
                }
                else                      //如果有人則先加入subQueue
                {
                    subQueue.Enqueue(myGroup);
                }

            }
        }
        /// <summary>
        /// 當是使用團進，並要取消排隊時呼叫
        /// </summary>
        /// <param name="uid"></param>
        public void CancelG(int uid)
        {
            lock ("mainQueueLock")
            {
                if (MainQueueContain(uid))
                    CancelQ(uid);
                else
                    waitingtobeD.Add(uid);
            }
        }
        /// <summary>
        /// set all the player = null and remove them at QueueorGaming
        /// </summary>
        /// <param name="captainuid"></param>
        /// <returns>remove success</returns>
        public bool ContainsAndMarkRemove(int captainuid)
        {
            lock ("mainQueueLock")
            {
                foreach (Baseplayer[] group in subQueue)
                {
                    if (group[0].uid == captainuid)
                    {
                        for (int i = 0; i < group.Length; i++)
                        {
                            group[i] = null;
                        }
                            return true;
                    }
                }
                return false;
            }
        }
        #endregion
    }


    #region 很多的Baseplayer集合。在多排時使用
    /// <summary>
    /// 很多的Baseplayer集合。在多排時使用
    /// </summary>
    public class Grouplayer
    {
        /// <summary>
        /// 如果不到五個的話，空的填null，不會存到list中
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="player3"></param>
        /// <param name="player4"></param>
        /// <param name="plauer5"></param>
        public Grouplayer(Baseplayer player1, Baseplayer player2, Baseplayer player3, Baseplayer player4, Baseplayer player5)
        {
            Group = new List<Baseplayer>();
            if (player1 != null)
                Group.Add(player1);
            if (player2 != null)
                Group.Add(player2);
            if (player3 != null)
                Group.Add(player3);
            if (player4 != null)
                Group.Add(player4);
            if (player5 != null)
                Group.Add(player5);
        }
        List<Baseplayer> Group;
    }
#endregion
}

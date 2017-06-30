using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace startOnline
{
    public class gamingCollection
    {
        /// <summary>
        /// 存放BaseRoom(遊戲房)的集合。
        /// </summary>
        public gamingCollection()
        {
            GamingRoom = new Dictionary<int,BaseRoom>();
        }
        Dictionary<int,BaseRoom> GamingRoom;

        /// <summary>
        /// 加入GamingRoom，並傳送訊息給在房裡的各玩家。 回傳成功加入房間的房號
        /// </summary>
        /// <param name="room">一個BaseRoom</param>
        public int Add(BaseRoom room)
        {
            int thisroomnumber = 0;
            lock ("gamingRoomLock")
            {
                thisroomnumber = this.ProductAbleRoomNumber();
                GamingRoom.Add(thisroomnumber,room);
            }
            return thisroomnumber;
        }
        #region 醫院程式碼
        /// <summary>
        /// 在遊戲房集合中有沒有這間房間
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <returns></returns>
        public bool Contain(int roomNumber)
        {
            lock ("gamingRoomLock")
            {
                return GamingRoom.ContainsKey(roomNumber);
            }
        }
        public void Remove(int roomNumber)
        {
            lock ("gamingRoomLock")
            {
                if (GamingRoom.ContainsKey(roomNumber))
                    GamingRoom.Remove(roomNumber);
            }
        }
        #endregion
        /// <summary>
        /// 回傳現在可的房號
        /// </summary>
        /// <returns></returns>
        public int ProductAbleRoomNumber()
        {
            lock ("gamingRoomLock")
            {
                int index = 1;
                foreach (KeyValuePair<int, BaseRoom> room in GamingRoom)
                {
                    if (room.Key != index)
                        return index;
                    index++;
                }
                return index;
            }
        }
    }
    /// <summary>
    /// 遊戲防，裡面有10位玩家
    /// </summary>
    public class BaseRoom
    {
        /// <summary>
        /// 10為玩家都要放，會將10 個 Baseplayer 存成一個陣列players，所以會從players[0]開始到players[9]
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="player3"></param>
        /// <param name="player4"></param>
        /// <param name="player5"></param>
        /// <param name="player6"></param>
        /// <param name="player7"></param>
        /// <param name="player8"></param>
        /// <param name="player9"></param>
        /// <param name="player10"></param>
        public BaseRoom(Baseplayer player1,Baseplayer player2,Baseplayer player3,Baseplayer player4,Baseplayer player5,Baseplayer player6,Baseplayer player7,Baseplayer player8,Baseplayer player9,Baseplayer player10)
        {
            players = new Baseplayer[]{player1,player2,player3,player4,player5,player6,player7,player8,player9,player10};
        }
        public Baseplayer[] players;

        /// <summary>
        /// 回傳在這個房間中除了自己以外玩家的UID List
        /// </summary>
        /// <param name="myuid">自己的uid</param>
        /// <returns>Uid list</returns>
        public List<int> GetOtherPlayerUIDInThisRoom(int myuid)
        {
            List<int> uidlist = new List<int>();
            foreach (Baseplayer player in players)
            {
                if (player.uid != myuid)
                    uidlist.Add(player.uid);
            }
            return uidlist;
        }
        /// <summary>
        /// 回傳在這個房間中除了自己以外玩家的UID List
        /// </summary>
        /// <param name="myuid">自己的在這個房間中的playerid</param>
        /// <returns>Uid list</returns>
        public List<int> GetOtherPlayerUIDInThisRoom(byte myplayerid)
        {
            List<int> uidlist = new List<int>();
            foreach (Baseplayer player in players)
            {
                if (player.playId != myplayerid)
                    uidlist.Add(player.uid);
            }
            return uidlist;
        }
        public List<gamePeer> GetOtherPlayerPeer(int myuid)
        {
            List<gamePeer> peerList = new List<gamePeer>();
            if (players != null)
            {
                foreach (Baseplayer player in players)
                {
                    if (player.uid != myuid)
                        peerList.Add(player.peer);
                }
            }
            return peerList;
        }
        /// <summary>
        /// 從 這個Peer 的 遊戲房暫存中，取得是第幾位
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetPlayerNumber(int uid)
        {
            int number = -1;
            if (players == null) //檢查有沒有東西
                return number;

            if (players.Length > 0)
            {
                int Number = 0;
                foreach (Baseplayer player in players)
                {

                    if (uid == player.uid)
                    {
                        return Number;
                    }
                    Number++;
                }
            }
            return number;
        }

    }
}

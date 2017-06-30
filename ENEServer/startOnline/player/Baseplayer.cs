using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace startOnline
{
    /// <summary>
    /// 基本的玩家單位。(其實就只是存玩家的屬性，隨時可新增)
    /// </summary>
    public class Baseplayer
    {
        /// <summary>
        /// 給予值
        /// </summary>
        /// <param name="uid">玩家的uid</param>
        /// <param name="name">玩家遊戲名子</param>
        public Baseplayer(int uid, string name,gamePeer peer)
        {
            this.uid = uid;
            this.name = name;
            this.peer = peer;
        }
        public int uid;
        public string name;
        //在遊戲房中是第幾位
        public byte playId;
        //是在哪一隊
        public byte teamNumber;
        public gamePeer peer;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace startOnline.Collections
{
    /// <summary>
    /// 只要有上線peer就會被加入。控管peer的集合
    /// </summary>
    public class myCollections
    {
        protected Dictionary<int, gamePeer> PeerClo {get;set;}
        public myCollections() 
        {
            PeerClo = new Dictionary<int, gamePeer>();
        }
        public gamePeer Getpeer(int uid)
        {
            if (PeerClo.ContainsKey(uid))
            {
                gamePeer p;
                PeerClo.TryGetValue(uid,out p);
                return p;
            }
            else
                return null;
        }
        ///<summary>
        /// 判斷有沒有在 線上集合裡
        ///</summary>
        public bool including(int uid)
        {
            if (PeerClo.ContainsKey(uid))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 將uid - peer 加入集合裡 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="p"></param>
        public void Addpeer(int uid, gamePeer p)
        {
            if (!PeerClo.ContainsKey(uid))
                PeerClo.Add(uid, p);
        }
        public void Deletepeer(int uid)
        {
            if (PeerClo.ContainsKey(uid))
                PeerClo.Remove(uid);
        }

    }
}

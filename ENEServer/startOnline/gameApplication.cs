using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using startOnline.Collections;
using startOnline.Rooms;
using startOnline.Queues;

namespace startOnline
{
    public class gameApplication : ApplicationBase
    {
        /// <summary>
        /// 存放peer，只要登入後就會被加入
        /// </summary>
        public myCollections Serverinfo;

        /// <summary>
        /// waiting teammates and enemy
        /// </summary>
        public Queue queue;

        /// <summary>
        /// GamingRooms Collection
        /// </summary>
        public List<GamingRoom> GamingRooms;

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new gamePeer(initRequest.Protocol, initRequest.PhotonPeer,this);
        }

        protected override void Setup()
        {
            CreateDBConnect();
            Serverinfo = new myCollections();
            GamingRooms = new List<GamingRoom>();
            queue = new Queue(2, 2,ref this.GamingRooms);

        }

        protected override void TearDown()
        {

        }

        #region 自己加的功能 Deliver
        /// <summary>
        /// 傳給 "指定一人"
        /// </summary>
        /// <param name="eventcode"></param>
        /// <param name="uid"></param>
        /// <param name="d"></param>
        public void Deliver(byte eventcode,int uid, Dictionary<byte, object> d) // 傳給 "指定一人"
        {
            if (Serverinfo.including(uid))
            {
                var eventData = new EventData(eventcode, d);
                Serverinfo.Getpeer(uid).SendEvent(eventData, new SendParameters());
            }
        }
        /// <summary>
        /// 傳給複數的人
        /// </summary>
        /// <param name="eventcode"></param>
        /// <param name="uid"></param>
        /// <param name="packet"></param>
        public void Deliver(byte eventcode,List<int> uidArray, Dictionary<byte, object> packet) 
        {
            foreach (int uid in uidArray)
            {
                if (Serverinfo.including(uid))
                {
                    var eventData = new EventData(eventcode, packet);
                    Serverinfo.Getpeer(uid).SendEvent(eventData, new SendParameters());
                }
            }
        }

        public void Deliver(byte eventcode, List<gamePeer> peers, Dictionary<byte, object> packet)
        {
            foreach (gamePeer peer in peers)
            {
                    var eventData = new EventData(eventcode, packet);
                    peer.SendEvent(eventData, new SendParameters());
            }
        }
        #endregion

        MySql.Data.MySqlClient.MySqlConnection conn;
        #region 資料庫相關功能
        public void CreateDBConnect()
        {

            string myConnectionString;
            myConnectionString = "server=localhost;uid=serveruser1;" + "pwd=t80529;database=start;";
            //serveruser1
            try
            {
                //conn = new MySql.Data.MySqlClient.MySqlConnection();
                //conn.ConnectionString = myConnectionString;
                conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
                conn.Open();
                MessageBox.Show("Connect Database Success");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Connect Database fail");
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
            }
        }
        public Dictionary<byte, object> Getgameinfo(string uid) //用uid取得 gameinfo 全部的 資料
        {
            Dictionary<byte,object> result = new Dictionary<byte,object>();
            string sql = "select * from start.gameinfo where uid = "+uid;
            MySqlCommand cmd1 = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    result.Add((byte)i, rdr[i].ToString());
                }
            }
            rdr.Close();
            return result;
        }
        public string Getgameinfo(string selectcolumn, string wherecolumn, string whereCondition) //select selectcolumn from start.gameinfo where wherecolumn = whereCondition;
        {
            string result="";
            string sql = "select "+ selectcolumn +" from start.gameinfo where "+ wherecolumn+" = "+whereCondition;
            //MessageBox.Show(sql);
            MySqlCommand cmd1 = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                result = rdr[0].ToString();
                //MessageBox.Show(rdr[0].ToString());
                break;
            }
            rdr.Close();
       
            return result;
        }

        public Dictionary<byte, object> Getfriendslist(string uid)  //用 uid 取得 friendslist 全部的 資料
        {
            Dictionary<byte, object> result = new Dictionary<byte, object>();
            string sql = "select * from start.friendslist where uid = " + uid;
            MySqlCommand cmd1 = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    result.Add((byte)i, rdr[i].ToString());
                }
            }
            rdr.Close();
            return result;
        }
        ///<summary>
        /// 取 Suid 裡面 Tuid 的 位置是在 friends幾
        ///</summary>
        public int GetfriendsNumber(string Suid,string Tuid)
        {
            int number = 0;
            string sql = "select * from start.friendslist where uid = " + Suid;
            MySqlCommand cmd1 = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    if (rdr[i].ToString().Equals(Tuid))
                        break;
                    number++;
                }
                break;
            }
            rdr.Close();
            if (number > 20)
                return -1;
            else
                return number;
        }
        public void Addfriends(string Suid, string Tuid)
        {
            Dictionary<byte, object> list = Getfriendslist(Tuid);
            Dictionary<byte, object> List = new Dictionary<byte, object>();
            int index=0;
            foreach(KeyValuePair<byte,object> item in list)
            {
                if (item.Value.ToString().Length > 0)
                    index = index + 1;
            }
            try
            {
                string sql = "UPDATE start.friendslist SET friend"+index.ToString()+"="+Suid+" where uid='"+Tuid.ToString()+"'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool UpdateColumn(string table,string selectcolumn, string wherecolumn, string whereCondition,string UpdateValue)
        {
            try 
            {
                string sql = "update start." + table + " set "+selectcolumn+" = "+UpdateValue+" where "+wherecolumn+" = "+whereCondition;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        ///<summary>
        /// 移除 Suid friendlist 裡的 Tuid (要為好友) ，並自動將最後面的補上
        ///</summary>
        public void Removefriend(string Suid,string Tuid)
        {
            Dictionary<byte, object> friendlist = Getfriendslist(Suid);
            byte friendMaxnumber = 0;
            foreach (KeyValuePair<byte, object> item in friendlist)
            {
                if (item.Value.ToString().Length>0&&item.Value.ToString()!=Suid)
                    friendMaxnumber++;
            }
            if (friendlist[friendMaxnumber].ToString().Equals(Tuid))
            {
                try
                {
                    string sql = "UPDATE start.friendslist SET friend" + friendMaxnumber + "=null where uid=" + Suid;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                string lessValue = friendlist[friendMaxnumber].ToString();
                int friendNumber = GetfriendsNumber(Suid, Tuid);
                try
                {
                    //先把friendNumber的那個用最後面的取代掉
                    string sql2 = "UPDATE start.friendslist SET friend" + friendNumber + "='"+lessValue+"' where uid=" + Suid;
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                    cmd2.ExecuteNonQuery();

                    string sql = "UPDATE start.friendslist SET friend" + friendMaxnumber + "=null where uid=" + Suid;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
           
        }
        public string GetNamefromUid(string uid)
        {
            string result="";
            string sql = "select name from start.gameinfo where uid="+uid;
            MySqlCommand cmd1 = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                result = rdr[0].ToString();
            }
            rdr.Close();
            return result;
        }
        /// <summary>
        /// DB方法_回傳Table_custominfo 中 有沒有存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        public bool including(string id, string psw) //in custominfo
        {
            string sql = "select count(*) from custominfo where custominfo.id='"+id+"' and custominfo.psw='"+psw+"'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int r = Convert.ToInt32(result);
                if (r == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool including(string sql) //直接給sql
        {
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int r = Convert.ToInt32(result);
                if (r == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public int GetUid(string id, string psw)
        {
            int uid = 0;
            string sql2 = "select uid from start.custominfo where id='" + id + "' and psw='" + psw + "' ";
            MySqlCommand cmd1 = new MySqlCommand(sql2, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                uid = Int32.Parse(rdr[0].ToString());
            }
            rdr.Close();
            return uid;
        }
        public int GetUid(string sql2)
        {
            int uid = 0;
            MySqlCommand cmd1 = new MySqlCommand(sql2, conn);
            MySqlDataReader rdr = cmd1.ExecuteReader();
            while (rdr.Read())
            {
                uid = Int32.Parse(rdr[0].ToString());
            }
            rdr.Close();
            return uid;
        }
        public bool includingGI(string uid)
        {
            string sql = "select count(*) from gameinfo where gameinfo.uid='" + uid + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int r = Convert.ToInt32(result);
                if (r == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        public bool IDincluding(string id)
        {
            string sql = "select * from start.custominfo where id='"+id+"'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int r = Convert.ToInt32(result);
                if (r >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool Emailincluding(string email)
        {
            string sql = "select * from start.custominfo where email='" + email + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int r = Convert.ToInt32(result);
                if (r >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //return 由資料庫產生的Uid
        public bool Register(int availableUid, string id, string psw, string email)
        {
            bool b = true;
            /*
            try
            {
                string sql = "insert into start.custominfo(uid,id,psw,email) values (" + availableUid + ",+'" + id + "','" + psw + "','" + email + "')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch { return false; }*/
            try
            {
                string sql = "insert into start.custominfo(uid,id,psw,email) values (" + availableUid + ",+'" + id + "','" + psw + "','" + email + "')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return b; 
        }
        public bool Register(string uid, string name)
        {
            bool b = true;
            try
            {
                string sql = "insert into start.gameinfo(uid,name) values (" + uid + ",+'" + name + "')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                string sql1 = "insert into start.friendslist(uid) values ("+uid+")";
                MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                cmd1.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return b;
        }
        public int AvailableUid()
        {
            try
            {
                string sql = "select uid from start.custominfo  order by uid DESC limit 1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) + 1;
            }
            catch { return 0; }
        }
        #region 測試
        /* 測試    
         *  測試  AvailableUid()
         *  
                string sql2 = "select uid from start.custominfo where id='" + id + "' and psw='" + psw + "' ";
                MySqlCommand cmd1 = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    uid = Int32.Parse(rdr[0].ToString());
                }
                rdr.Close();
         * 
public void text(string id, string psw, string email)
{
    try
    {
        MessageBox.Show(AvailableUid().ToString());
        string sql = "insert into start.custominfo(uid,id,psw,email) values (" + AvailableUid().ToString() + ",+'" + id + "','" + psw + "','" + email + "')";
        MySqlCommand cmd = new MySqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
    catch
    { MessageBox.Show("失敗"); }
}
  
         測試MySqlDataReader
 * public void test()
{
    try
    {
        string sql2 = "select uid from start.custominfo where id='gihhgihh1' and psw='t80529' ";
        MySqlCommand cmd1 = new MySqlCommand(sql2, conn);
        MySqlDataReader rdr = cmd1.ExecuteReader();

        while (rdr.Read())
        {
            MessageBox.Show( rdr[0].ToString());
        }
        rdr.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}
 * */
        #endregion

        #endregion
        /// <summary>
        /// 為某位玩家增加錢(energy)，直接在資料庫裡修改
        /// </summary>
        /// <param name="uid">要修改玩家的UID</param>
        /// <param name="increase_account">要增加多少</param>
        public void Increase_Money(int uid, int increase_account)
        {
            int now_money = Int32.Parse(Getgameinfo("energy", "uid", uid.ToString()));
            now_money += increase_account;
            UpdateColumn("gameinfo", "energy", "uid", uid.ToString(), now_money.ToString());
        }
    }
}

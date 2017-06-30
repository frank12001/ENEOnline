using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Windows.Forms;
using startOnline.Collections;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using startOnline.Rooms;

namespace startOnline
{
    public class gamePeer : PeerBase
    {
        private gameApplication _server;
        private int peerUID=-1;
        public int Uid { get { return this.peerUID; } }
        public int whichSceneThePlayerIn=0;

        #region GamingRoom 用的參數,存在這邊減少傳輸封包大小
        public byte PlayerNumber;
        #endregion


        public gamePeer(IRpcProtocol rpcProtocol, IPhotonPeer nativePeer,gameApplication app)
            : base(rpcProtocol, nativePeer)
        {
            _server = app;
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            #region 下線後，傳給在現好友，要求更改狀態
            if (!this.peerUID.Equals(-1))
            {
                Dictionary<byte, object> friendslist = _server.Getfriendslist(this.peerUID.ToString());
                Dictionary<int, object> result2 = new Dictionary<int, object>();
                foreach (KeyValuePair<byte, object> item in friendslist) //用uid取得名子
                {
                    if (item.Value.ToString().Length > 0)
                    {
                        result2.Add(Int32.Parse(item.Value.ToString()), _server.GetNamefromUid(item.Value.ToString()));
                        //index++;
                    }
                }
                foreach (KeyValuePair<int, object> item in result2)
                {
                    if (!item.Key.Equals(this.peerUID) && _server.Serverinfo.including(item.Key))
                    {
                        Dictionary<byte, object> packet5 = new Dictionary<byte, object> { { (byte)0, this.peerUID },{(byte)1,1}, };
                        _server.Deliver((byte)5, item.Key, packet5);
                    }
                }
            }
            #endregion

            //MessageBox.Show("uid {0} is leave", peerUID.ToString());
            _server.Serverinfo.Deletepeer(this.peerUID);
            this.Flush();
                   
        }
        public delegate void ReceiveDictionaryHandler(Dictionary<byte,object> packet);
        public event ReceiveDictionaryHandler PrepareRoom_Messages; //In PrepareRoom.cs
        public event ReceiveDictionaryHandler GamingRoom_Messages; //In GamingRoom.cs
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
           switch (operationRequest.OperationCode)
           {
               #region 0 Test (not use now)
               case (byte)OperationCode.Test:
                   break;
               #endregion
               #region 1 登入
               //登入
               case (byte)1:
                   var memberID = (string)operationRequest.Parameters[1];
                   var memberPW = (string)operationRequest.Parameters[2];

                   if (_server.including(memberID, memberPW))
                   {
                       int uid = _server.GetUid(memberID, memberPW);
                       Dictionary<byte, object> dic = new Dictionary<byte, object> { { (byte)0, uid }, { (byte)1, memberID }, { (byte)2, memberPW }, };
                       //OperationResponse respone = new OperationResponse(operationRequest.OperationCode, dic) { ReturnCode = (short)0 };
                       //SendOperationResponse(respone, new SendParameters());
                       if (!_server.Serverinfo.including(uid))
                       {
                           _server.Serverinfo.Addpeer(uid, this);
                           OperationResponse respone = new OperationResponse(operationRequest.OperationCode, dic) { ReturnCode = (short)0 };
                           SendOperationResponse(respone, new SendParameters());
                           this.peerUID = uid; //將 uid 記在這個peer上
                           
                       }
                       else
                       {
                           OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)2 };
                           SendOperationResponse(respone, new SendParameters());
                       }
                   }
                   else
                   {
                       OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                       SendOperationResponse(respone, new SendParameters());
                   }
                   break;
               #endregion 
               #region 2 註冊
               //註冊
               case (byte)2:           
                       var ID = (string)operationRequest.Parameters[1];
                       var PW = (string)operationRequest.Parameters[2];
                       var Email = (string)operationRequest.Parameters[3];

                       byte key = 0;
                       if (!_server.IDincluding(ID))
                       { key = 1; }
                       if (!_server.Emailincluding(Email)&&key==1)
                       { key = 2; }
                       if (key == 0) //檢查失敗 ，2個檢查都沒過
                       {
                           OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                           SendOperationResponse(respone, new SendParameters());
                       }
                       else if (key == 1)
                       {
                           OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)2 };
                           SendOperationResponse(respone, new SendParameters());
                       }
                       else if (key == 2) //成功通過檢查
                       {
                           int uid = _server.AvailableUid();
                           if (_server.Register(uid, ID, PW, Email))
                           {
                               Dictionary<byte, object> dic = new Dictionary<byte, object> 
                               {
                                   {(byte)0,uid},
                                   {(byte)1,ID},
                                   {(byte)2,PW},
                               };
                               OperationResponse respone = new OperationResponse(operationRequest.OperationCode, dic) { ReturnCode = (short)0 };
                               SendOperationResponse(respone, new SendParameters());
                           }
                           else //通過檢查，在將資料寫入時發生錯誤
                           {
                               OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)3 };
                               SendOperationResponse(respone, new SendParameters());
                           }
                       }
                   break;
               #endregion              
               #region 3 S2check
               case 3: //判斷是不是第一次登入
                   if (_server.includingGI(operationRequest.Parameters[1].ToString()))
                   {
                       OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)0 };
                       SendOperationResponse(respone, new SendParameters());
                   }
                   else
                   {
                       OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                       SendOperationResponse(respone, new SendParameters());
                   }
                   break;
               #endregion
               #region 4 S21 check "join name"
               case 4:
                   var uid1 = operationRequest.Parameters[0].ToString();
                   var name = (string)operationRequest.Parameters[1];
                   string sql = "select count(*) from start.gameinfo where name='"+name+"'";
                   if (_server.including(sql))
                   {
                       OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                       SendOperationResponse(respone, new SendParameters());
                   }
                   else
                   {
                       if (_server.Register(uid1, name))
                       {
                           OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)0 };
                           SendOperationResponse(respone, new SendParameters());
                       }

                       else
                       {
                           OperationResponse respone = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                           SendOperationResponse(respone, new SendParameters());
                       }
                   }
                   break;
               #endregion
               #region 5 S3 getGameinfo + getfriendsname + 製作 好友狀態陣列
               case 5:
                   var uid2 = operationRequest.Parameters[0].ToString();
                   byte fromWhichScene = byte.Parse(operationRequest.Parameters[1].ToString()) ; 
                   Dictionary<byte, object> gameinfo = _server.Getgameinfo(uid2);
                   Dictionary<byte, object> friendslist = _server.Getfriendslist(uid2);
                   Dictionary<int, object> result2 = new Dictionary<int, object>();
                   Dictionary<int, bool> friendsstate = new Dictionary<int, bool>();
                   //byte index=0;
                   foreach (KeyValuePair<byte, object> item in friendslist) //用uid取得名子
                   {
                       if (item.Value.ToString().Length > 0)
                       {
                           result2.Add( Int32.Parse(item.Value.ToString()), _server.GetNamefromUid(item.Value.ToString()));
                           //index++;
                       }
                   }
                   foreach (KeyValuePair<byte, object> item in friendslist) //用uid取得state
                   {
                       if (item.Value.ToString().Length > 0)
                       {
                          friendsstate.Add(Int32.Parse(item.Value.ToString()), _server.Serverinfo.including(int.Parse(item.Value.ToString())));
                       }
                   }
                   Dictionary<byte, object> result = new Dictionary<byte, object>
                   {
                       {(byte)1,gameinfo},
                       {(byte)2,friendsstate},
                       {(byte)3,result2},
                   };
                   OperationResponse respone1 = new OperationResponse(operationRequest.OperationCode, result) { ReturnCode = (short)0 };
                   SendOperationResponse(respone1, new SendParameters());
                   //上線之後，跟其他在線好友說我上現了
                   //寫在這裡 //maybe problem in here
                   if (fromWhichScene == 3)
                   {
                       foreach (KeyValuePair<int, object> item in result2)
                       {
                           if (!item.Key.Equals(uid2) && _server.Serverinfo.including(item.Key))
                           {
                               Dictionary<byte, object> packet5 = new Dictionary<byte, object> { { (byte)0, uid2 }, { (byte)1, 0 }, };
                               _server.Deliver((byte)5, item.Key, packet5);
                           }
                       }
                   }

                   whichSceneThePlayerIn = 3;
                   break;
               
               #endregion
               #region 6 S3 getfriendsname Addfriend-第一關
               case 6:  //addfriend1
                   //MessageBox.Show("in case 6");
                   var Suid =Int32.Parse(operationRequest.Parameters[0].ToString());
                   var Sname = operationRequest.Parameters[1].ToString();
                   var Tname = operationRequest.Parameters[2].ToString();
                   string sfind = "select count(*) from start.gameinfo where gameinfo.name = '"+Tname+"'";
                   //MessageBox.Show(Sname+"//"+Tname.ToString());
                   if (_server.including(sfind))
                   {
                       string sql2 = "select uid from start.gameinfo where gameinfo.name = '"+Tname+"'";
                       int Tuid = _server.GetUid(sql2);
                       if (_server.Serverinfo.including(Tuid))
                       {
                           Dictionary<byte,object> d = new Dictionary<byte,object>
                           {
                               {(byte)2,Suid},
                               {(byte)3,Sname},
                           };
                           //MessageBox.Show("send///"+d[2].ToString());
                           _server.Deliver((byte)6, Tuid, d);
                           OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)0 };
                           SendOperationResponse(response, new SendParameters());
                       }
                       else
                       {
                           //該玩家不在線上
                           OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)2 };
                           SendOperationResponse(response, new SendParameters());
                       }
                   }
                   else
                   {  //該玩家不存在
                       OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                       SendOperationResponse(response, new SendParameters());
                   }
                   //MessageBox.Show(Tname.ToString());
                   break;
               #endregion
               #region 8 S3 addfriend3 Addfriend-第二關
               case 8:
                   //MessageBox.Show("in case 8");
                   var Suid1 = Int32.Parse(operationRequest.Parameters[0].ToString());
                   var Tuid1 = Int32.Parse(operationRequest.Parameters[1].ToString());
                   bool agree = bool.Parse(operationRequest.Parameters[2].ToString());
                   var Sname1 = operationRequest.Parameters[3].ToString();
                     //MessageBox.Show(Suid1.ToString() + "///" + Tuid1.ToString() + "///" + agree.ToString());
                   
                   if (agree)
                   {
                       _server.Addfriends(Suid1.ToString(), Tuid1.ToString()); //兩邊都要加入
                       _server.Addfriends(Tuid1.ToString(), Suid1.ToString());
                   }
                   Dictionary<byte, object> packet = new Dictionary<byte, object>()
                   {
                       {(byte)1,Suid1},
                       {(byte)2,Sname1},
                       {(byte)3,agree},
                   };
                   _server.Deliver((byte)8, Tuid1, packet);
                   //MessageBox.Show("inin");
                   break;
               #endregion
               #region 7 S3 friendslist's buttonMouseDown 
               case 7:  
                   //MessageBox.Show("in7");
                   var Suid7 = operationRequest.Parameters[0];
                  //MessageBox.Show(Suid7.ToString());
                   var Sname7 = operationRequest.Parameters[1];
                 //MessageBox.Show(Sname7.ToString());
                   var Tuid7 = operationRequest.Parameters[2];
                 //MessageBox.Show(Tuid7.ToString());
                   var Tname7 = operationRequest.Parameters[3];
                 //MessageBox.Show(Tname7.ToString());
                   if (_server.Serverinfo.including(int.Parse(Tuid7.ToString())))
                   {
                       //Dictionary<byte, object> d = new Dictionary<byte, object> { { (byte)0, Suid7 },{(byte)1,Sname7}, };
                       
                       //_server.Deliver((byte)7, int.Parse(Tuid7.ToString()), d);

                       Dictionary<byte, object> d2 = new Dictionary<byte, object> { { (byte)0, Tuid7 }, { (byte)1, Tname7 }, };
                       OperationResponse response = new OperationResponse(operationRequest.OperationCode,d2) { ReturnCode = (short)0 };
                       SendOperationResponse(response, new SendParameters());
                       //還要用Sendevent發給 Tuid
                   }
                   else
                   {
                       ////測時時使用這裡
                       Dictionary<byte, object> d2 = new Dictionary<byte, object> { { (byte)0, Tuid7 }, { (byte)1, Tname7 }, };
                       OperationResponse response = new OperationResponse(operationRequest.OperationCode, d2) { ReturnCode = (short)1 };
                       SendOperationResponse(response, new SendParameters());
                       ////------------
                       //----測試完換回來，以下為正確的
                       //OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                       //SendOperationResponse(response, new SendParameters());
                   }
                   break;
               #endregion
               #region 9 S3 刪除好友
               case (byte)9: 
                   //MessageBox.Show("in 9");
                   var Suid9 = Int32.Parse(operationRequest.Parameters[0].ToString());
                   var Tuid9 = Int32.Parse(operationRequest.Parameters[1].ToString());
                   var Tname9 = operationRequest.Parameters[2].ToString();
                   //MessageBox.Show(_server.Getgameinfo("uid","name","'"+Tname9.ToString()+"'"));
                   //_server.Removefriend(Suid9.ToString(), Tuid9.ToString());
                   try
                   {
                       _server.Removefriend(Suid9.ToString(), Tuid9.ToString());
                       _server.Removefriend(Tuid9.ToString(), Suid9.ToString());
                       //---先把要回傳給發送這的Dic做好
                       Dictionary<byte, object> friendslist9 = _server.Getfriendslist(Suid9.ToString());
                       Dictionary<int, object> result9 = new Dictionary<int, object>();
                       foreach (KeyValuePair<byte, object> item in friendslist9) //用uid取得名子
                       {
                           if (item.Value.ToString().Length > 0)
                           {
                               result9.Add(Int32.Parse(item.Value.ToString()), _server.GetNamefromUid(item.Value.ToString()));
                               //index++;
                           }
                       }
                       Dictionary<int, bool> friendsstate9 = new Dictionary<int, bool>();
                       foreach (KeyValuePair<byte, object> item in friendslist9) //用uid取得state
                       {
                           if (item.Value.ToString().Length > 0)
                           {
                               friendsstate9.Add(Int32.Parse(item.Value.ToString()), _server.Serverinfo.including(int.Parse(item.Value.ToString())));
                           }
                       }
                       Dictionary<byte, object> result91 = new Dictionary<byte, object>
                           {
                               {(byte)2,friendsstate9},
                               {(byte)3,result9},
                           };
                       //----
                       if (!_server.Serverinfo.including(Tuid9)) //若他不在線上時
                       {   
                           //直接回傳一邊的
                           OperationResponse response = new OperationResponse(operationRequest.OperationCode,result91) { ReturnCode = (short)0 };
                           SendOperationResponse(response, new SendParameters());
                       }
                       else   //若在線上時
                       {
                           //做SendEvent 的字典
                           Dictionary<byte, object> Tfriendslist9 = _server.Getfriendslist(Tuid9.ToString());
                           Dictionary<int, object> Tresult9 = new Dictionary<int, object>();
                           foreach (KeyValuePair<byte, object> item in Tfriendslist9) //用uid取得名子
                           {
                               if (item.Value.ToString().Length > 0)
                               {
                                   Tresult9.Add(Int32.Parse(item.Value.ToString()), _server.GetNamefromUid(item.Value.ToString()));
                                   //index++;
                               }
                           }
                           Dictionary<int, bool> Tfriendsstate9 = new Dictionary<int, bool>();
                           foreach (KeyValuePair<byte, object> item in Tfriendslist9) //用uid取得state
                           {
                               if (item.Value.ToString().Length > 0)
                               {
                                   Tfriendsstate9.Add(Int32.Parse(item.Value.ToString()), _server.Serverinfo.including(int.Parse(item.Value.ToString())));
                               }
                           }
                           Dictionary<byte, object> Tresult91 = new Dictionary<byte, object>
                           {
                               {(byte)2,Tfriendsstate9},
                               {(byte)3,Tresult9},
                           };
                           //foreach(KeyValuePair<int,object> )
                           OperationResponse response = new OperationResponse(operationRequest.OperationCode,result91) { ReturnCode = (short)0 };
                           SendOperationResponse(response, new SendParameters());

                           _server.Deliver((byte)9, Tuid9, Tresult91);
                       }
                   }
                   catch //發生無法預期的錯誤，回傳刪除失敗
                   {
                       OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                       SendOperationResponse(response, new SendParameters());
                   }
                   break;
               #endregion
               #region 10 S3 在mainchat 按下Enter後，傳入 處理聊天字串的傳送
               case 10:
                   //MessageBox.Show("in10");
                   var Suid10 =operationRequest.Parameters[0];
                   //MessageBox.Show(Suid10.ToString());
                   var Sname10 = operationRequest.Parameters[1];
                   //MessageBox.Show(Sname10.ToString());
                   var Tuid10 = operationRequest.Parameters[2];
                   //MessageBox.Show(Tuid10.ToString());
                   var Tname10 = operationRequest.Parameters[3];
                   //MessageBox.Show(Tname10.ToString());
                   var Sstring10 = operationRequest.Parameters[4];
                   //MessageBox.Show(Sstring10.ToString());
                   Dictionary<byte, object> packet10 = new Dictionary<byte, object> 
                   {
                       {(byte)0,Suid10},
                       {(byte)1,Sname10},
                       {(byte)2,Tuid10},
                       {(byte)3,Tname10},
                       {(byte)4,Sstring10},
                   };
                   _server.Deliver((byte)10, int.Parse(Tuid10.ToString()), packet10);
                   break;
               #endregion
               #region 11 S3 SingleQ
               case 11:
                   byte switchCode = (byte)operationRequest.Parameters[0];
                   switch (switchCode)
                   {            
                       case 1: //Queuing Resquest
                             if(!_server.queue.IsThisPlayerInQueue(this))
                                 _server.queue.AddPlayerInQueue(this);
                             break;
                       case 2:
                             if (_server.queue.RemoveThePlayerInQueue(this))
                             {
                                 OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)1 };
                                 SendOperationResponse(response, new SendParameters());
                             }
                             else
                             {
                                 OperationResponse response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)2 };
                                 SendOperationResponse(response, new SendParameters());
                             }
                             break;
                   }
                   break;
               #endregion
               #region S3 PrepareRoom Messages
               case 12 :
                   PrepareRoom_Messages(operationRequest.Parameters);
                   break;
               #endregion
               #region S4 GamingRoom Messages
               case 13:
                   operationRequest.Parameters.Add((byte)124, this.PlayerNumber); //每次有封包進來都先將PlayerNumber給它
                   GamingRoom_Messages(operationRequest.Parameters);
                   break;
               #endregion

           }
        }
        public void Increase_Money(int increase_account)
        {
            _server.Increase_Money(this.peerUID, increase_account);
        }
        /// <summary>
        /// 移除遊戲房
        /// </summary>
        /// <param name="room">要移除的遊戲房</param>
        public void RemoveGamingRoom(GamingRoom room)
        {
            _server.GamingRooms.Remove(room);
        }

        public void SendEvent(byte eventCode, Dictionary<byte, object> packet)
        {
            EventData eventData = new EventData((byte)eventCode, packet);
            this.SendEvent(eventData, new SendParameters());
        }

        public byte[] SerializeToByteArray(object source)
        {
            var Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new System.IO.MemoryStream()) 
            {
                Formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
               // MessageBox.Show();
                Formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }
        public object ToObject(byte[] source)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new MemoryStream((byte[])source))
            {
                formatter.Binder = new CurrentAssemblyDeserializationBinder();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                return formatter.Deserialize(stream);
            }
            
        }
        /// <summary>
        /// 新增一個執行緒 並用它新增一個MessageBox
        /// </summary>
        /// <param name="context"></param>
        private void DisplayMessageBox(string context)
        {
            new Thread(new ThreadStart(delegate
            {
                MessageBox.Show(context);
            })).Start();
        }
       
    }

}
public sealed class CurrentAssemblyDeserializationBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        return Type.GetType(String.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName));
    }
}

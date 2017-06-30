using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;

namespace startOnline.Rooms
{
    public class PrepareRoom : IDisposable
    {
        public PrepareRoom(gamePeer[] peers, byte howMuchTeamsInAGame)
        {
            if(peers.Length%howMuchTeamsInAGame!=0)
                throw new Exception("Each Team members isn't same or player's number problem.");
            if(peers.Length>125)
                throw new Exception("Players can't > 125 ");
            this.howMuchTeamsInAGame = howMuchTeamsInAGame;
            this.howMuchPlayersInAGame = (byte)peers.Length;
            this.howMuchMembersInATeam = (byte)(howMuchPlayersInAGame/howMuchTeamsInAGame);

            this.teams = new List<gamePeer[]>();
            this.Result_HerosIdArray = new List<byte[]>();
            for (int i = 0; i < howMuchTeamsInAGame; i++)
            {
                teams.Add(new gamePeer[howMuchMembersInATeam]);
                Result_HerosIdArray.Add(new byte[howMuchMembersInATeam]);
            }
            byte member = 0;
            sbyte team = -1;
            for (int i = 0; i < howMuchPlayersInAGame; i++)
            {
                if (i % howMuchMembersInATeam == 0)
                {
                    member = 0;
                    team++;
                }
                teams[team][member] = peers[i];
                member++;
            }
            /*---send message to client tell them , they are in the prepare room now--*/
            for(int i=0;i<teams.Count;i++)
            {
                for(int j=0;j<teams[i].Length;j++)
                {
                    teams[i][j].PrepareRoom_Messages += this.PrepareRoom_Messages;
                    Dictionary<byte,object> player = new Dictionary<byte,object>
                    {
                        {(byte)0,(byte)i},
                        {(byte)1,(byte)j},
                    };
                    Dictionary<byte, object> packet = new Dictionary<byte, object> { { (byte)packetCode.switchCode, (byte)MessagesCatrgroies.Joining },
                                                                                      {(byte)packetCode.whoAmI,player},
                                                                                   };
                    SendPacketToAssignPlayer(packet,player);
                }
            }
            /*-------*/
            

        }

        private byte howMuchPlayersInAGame;
        private byte howMuchTeamsInAGame;
        private byte howMuchMembersInATeam;
        public List<gamePeer[]> teams;
        private enum packetCode :byte { switchCode=0,whoAmI,NumberOfTeam,MembersInATeam,heroid, }
        private enum MessagesCatrgroies :byte
        {
            Joining = 1,
            ChangeHeroImage=2,
            Result = 3,
        }
        public List<byte[]> Result_HerosIdArray; //the result
        private byte index = 0;
        /// <summary>
        /// check the value in Result_HerosIdArray
        /// </summary>
        /// <returns></returns>
        private bool checkRseultList()
        {
            foreach (byte[] i in Result_HerosIdArray)
            {
                foreach (byte result in i)
                {
                    if (result == null)
                        return false;
                }
            }
            return true;
        }

        public delegate void resultProcesser_PrepareRoom(PrepareRoom preparerRoom); //Parameter is designed
        public event resultProcesser_PrepareRoom resultProcesser_prepareRoom;
        /*event*/
        private void PrepareRoom_Messages(Dictionary<byte, object> packet)
        {
            //op case 12
            /*-------*/
            byte switchcode = (byte)packet[(byte)packetCode.switchCode]; 
            switch (switchcode)
            {
                case (byte)MessagesCatrgroies.Result:
                    index++;
                    /*receive player's resule messages*/
                    byte heroid = (byte)packet[(byte)packetCode.heroid];
                    Dictionary<byte, object> whoAmI = (Dictionary<byte, object>)packet[(byte)packetCode.whoAmI];
                    byte team = (byte)whoAmI[0];
                    byte Members = (byte)whoAmI[1];
                    Result_HerosIdArray[team][Members] = heroid;
                    /*--Send result to every client to chahgeHero's Image--*/
                    Dictionary<byte,object> player = new Dictionary<byte,object>
                    {
                        {(byte)0,team},
                        {(byte)1,Members},
                    };
                    Dictionary<byte, object> result_changeHero = new Dictionary<byte, object> 
                    {
                        {(byte)packetCode.switchCode,(byte)MessagesCatrgroies.ChangeHeroImage},
                        {(byte)packetCode.whoAmI,player},
                        {(byte)packetCode.heroid,(byte)heroid},
                    };
                    SendPacketToEveryPlayers(result_changeHero);
                    /*-if(xxxx)-*/
                    if(index==howMuchPlayersInAGame)
                       this.resultProcesser_prepareRoom(this);
                    /*--*/                   
                    break;
            }
        }
        public void Dispose()
        {
            /*--need to been called when this room close--*/

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = 0; j < teams[i].Length; j++)
                {
                    teams[i][j].PrepareRoom_Messages -= this.PrepareRoom_Messages;                    
                }
            }
        }
        //connect with client
        public void SendPacketToEveryPlayers(Dictionary<byte, object> packet)
        {
            EventData data = new EventData((byte)EventCode.SendPrepareRoomMessage,packet);
            foreach (gamePeer[] players in teams)
            {
                foreach (gamePeer player in players)
                {
                    player.SendEvent(data, new SendParameters());
                }
            }
        }
        public void SendPacketToAssignPlayer(Dictionary<byte, object> packet,Dictionary<byte,object> whoAmI)
        {
            EventData data = new EventData((byte)EventCode.SendPrepareRoomMessage, packet);
            teams[(byte)whoAmI[0]][(byte)whoAmI[1]].SendEvent(data, new SendParameters());
        }

        
    }
}

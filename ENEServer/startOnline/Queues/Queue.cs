using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Photon.SocketServer;
using startOnline;
using startOnline.Rooms;

namespace startOnline.Queues
{
    public class Queue
    {
        public Queue(byte whichNumberForATeam, byte howMuchTeamsInAGame,ref List<GamingRoom> gamingRooms)
        {
            this.TeamMemberAccount = whichNumberForATeam;
            this.howMuchTeamsInAGame = howMuchTeamsInAGame;
            this.queue = new List<gamePeer>();
            this.prepareRooms = new List<PrepareRoom>();
            this.gamingRooms = gamingRooms;
        }

        private byte TeamMemberAccount;
        private byte howMuchTeamsInAGame;
        private List<gamePeer> queue;
        private List<PrepareRoom> prepareRooms;
        private List<GamingRoom> gamingRooms;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <returns>false = join fail</returns>
        public bool? AddPlayerInQueue(gamePeer peer)
        {
            lock ("Queuing")
            {
                if (IsThisPlayerInQueue(peer))
                {
                    //exception process
                    Dictionary<byte, object> packet = new Dictionary<byte, object> { { (byte)0, (int)ExceptionCode.RepeatQueue }, };
                    EventData data = new EventData((byte)EventCode.DeliverExceptionToClient, packet);
                    peer.SendEvent(data, new SendParameters());

                    RemoveThePlayerInQueue(peer);
                    return false;
                }
                else 
                {
                    int totalPlayers = TeamMemberAccount * howMuchTeamsInAGame;
                    queue.Add(peer);
                    if (queue.Count != totalPlayers) //if players which in the queue isn't meet requirements for gaming
                    {
                        // join queue success, waiting for join prepareRoom(waiting gaming)
                        return true;
                    }
                    else
                    {
                        foreach (gamePeer player in queue) //check player , Are they in the correct scene
                        {
                            if (player.whichSceneThePlayerIn != 3)
                            {
                                Dictionary<byte, object> packet = new Dictionary<byte, object> { {(byte)0,(int)ExceptionCode.InTheIncorrectSceneWhenReadyToPrepareRoom}, };
                                EventData data = new EventData((byte)EventCode.DeliverExceptionToClient, packet);
                                peer.SendEvent(data, new SendParameters());
                                RemoveThePlayerInQueue(player);
                                if (player.Uid == peer.Uid) 
                                    return false;
                            }
                        }
                        //------//
                        PrepareRoom prepareRoom;
                        gamePeer[] peers = new gamePeer[totalPlayers];
                        queue.CopyTo(peers);
                        queue.Clear();
                        prepareRoom = new PrepareRoom(peers, howMuchTeamsInAGame);
                        prepareRoom.resultProcesser_prepareRoom += this.resultProcesser_PrepareRoom; /*hook up because the function "resultProcesser_PrepareRoom" should in other Class*/
                        this.prepareRooms.Add(prepareRoom);
                        return null;
                    }
                }
            }
        }
        /*-operate queue function-*/
        public bool IsThisPlayerInQueue(gamePeer peer)
        {
            lock ("Queuing")
            {
                foreach (gamePeer player in this.queue)
                {
                    if (peer.Uid == player.Uid)
                        return true;
                }
                return false;
            }
        }
        private bool isThisPlayerInQueue(gamePeer peer)
        {
            foreach (gamePeer player in this.queue)
            {
                if (peer.Uid == player.Uid)
                    return true;
            }
            return false;
        }


        public bool RemoveThePlayerInQueue(gamePeer peer)
        {
            lock ("Queuing")
            {
                if (isThisPlayerInQueue(peer))
                {
                    for (int i = 0; i < queue.Count; i++)
                    {
                        if (peer.Uid == queue[i].Uid)
                        {
                            queue.RemoveAt(i);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        /*-------*/
        /*-the event to Process the result for PrepareRoom-*/
        private void resultProcesser_PrepareRoom(PrepareRoom prepareRoom)
        {
            /*--create GamingRoom And Remove prepareRoom--*/
            GamingRoom gamingRoom = new GamingRoom(prepareRoom.teams, prepareRoom.Result_HerosIdArray);
            this.gamingRooms.Add(gamingRoom);
            prepareRoom.Dispose();
            this.prepareRooms.Remove(prepareRoom);
        }
    }
}

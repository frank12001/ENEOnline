using UnityEngine;
using System.Collections.Generic;

public class chatS4 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ConnectFunction.F.chatReceiver += this.chatReceiver;
	}

    void OnDestroy(){
        ConnectFunction.F.chatReceiver -= this.chatReceiver;
    }

    public GameObject input, output;
    int chatstate = 0;
    int myplayerNumber=-1;
    GameObject inputT;
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return) && chatstate == 0)
        {

            Vector3 height = new Vector3(0.0f, 1.072f, 0.0f);
            if(myplayerNumber==-1)
               myplayerNumber = playerGroup.GetPlayerNumber((int)(ConnectFunction.F.playerInfo.UID));
            if (myplayerNumber >= 5)
                myplayerNumber = myplayerNumber - 5;
            if (myplayerNumber != -1)
            {

                inputT = (GameObject)Instantiate(input, initialize.bugcontroller[myplayerNumber].gameObject.transform.position + height, input.transform.rotation);
                inputT.GetComponent<input>().Focus();
                chatstate = 1;
            }

            
        }
        else if (Input.GetKeyDown(KeyCode.Return) && chatstate == 1)
        {
           
            chatstate = 0;


            string chatcontect = inputT.GetComponent<input>().GetInputText(); //input.xxxxx
            Debug.Log(chatcontect);
            Dictionary<byte, int> targetList = new Dictionary<byte, int>();
            byte index = 0;
            foreach (KeyValuePair<int, Baseplayer> player in playerGroup.playersGroup)
            {
                if (playerGroup.IsTeammate(player.Value.playId))
                    targetList.Add(index,player.Value.uid);
                index++;
            }
            Dictionary<byte, object> packet = new Dictionary<byte, object> 
            {
                {(byte)1,chatcontect},
                {(byte)2,myplayerNumber},
                {(byte)3,targetList},
            };
            ConnectFunction.F.Deliver((byte)14, packet);
            Destroy(inputT.gameObject);
            
        }
          
            
	}
    public void chatReceiver(Dictionary<byte, object> packet)
    {
        //這裡擺接收的事件 = 顯示聊天內容在Sender那隻蟲的上方
        var SplayerID =int.Parse(packet[1].ToString());
        Vector3 height = new Vector3(0.0f, 1.072f, 0.0f);
        GameObject chat = (GameObject) Instantiate(output, initialize.bugcontroller[SplayerID].gameObject.transform.position + height, output.transform.rotation);
        string chatContent = packet[2].ToString();
        chat.GetComponent<output>().SetText(chatContent);
    }
}

using UnityEngine;
using System.Collections.Generic;

public class GroupModeMainScript : GroupModeBase{

    public GameObject button1;

    public GameObject SingleQ, ComplexQ;
    public void OnEnable()
    {
        //ConnectFunction.F.playerInfo.UID  DataManager.datapool.name
        try
        {
            Baseplayer me = new Baseplayer((int)ConnectFunction.F.playerInfo.UID, DataManager.datapool.name);
            players[0] = me;
            Debug.Log("myuid " + me.uid + " name  " + me.name);
        }
        catch { }
        SingleQ.SetActive(false);
        ComplexQ.SetActive(false);
        TriangleButton.GroupMode = true;
    }

    public void OnDisable()
    {
        for (int i=0; i < players.Length; i++)
        {
            players[i] = null;
        }
        SingleQ.SetActive(true);
        ComplexQ.SetActive(true);
        TriangleButton.GroupMode = false;
    }
}

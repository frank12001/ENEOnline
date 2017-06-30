using UnityEngine;
using System.Collections;

public class GroupModePlayer : GroupModeBase{
    
    //這個Mode 只藉由 Queuedelegate 打開
    public GameObject button2;

    public GameObject SingleQ, ComplexQ;
	// Use this for initialization
	void Start () {
        base.Start();
        this.gameObject.SetActive(false);
	}
    public void OnEnable()
    {
        TriangleButton.GroupModePlayer = true;
        SingleQ.SetActive(false);
        ComplexQ.SetActive(false);
        TriangleButton.GroupModePlayer = true;
    }

    public void OnDisable()
    {
        TriangleButton.GroupModePlayer = false;
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = null;
        }
        SingleQ.SetActive(true);
        ComplexQ.SetActive(true);
        TriangleButton.GroupModePlayer = false;
    }

    
}

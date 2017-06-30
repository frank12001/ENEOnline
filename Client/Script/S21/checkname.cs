using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class checkname : MonoBehaviour {
    public GameObject g,text;
    public void OnMouseDown()
    {
        string s = g.GetComponent<InputField>().text;
        if (s.Length > 0)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>
                {
                    {(byte)0,ConnectFunction.F.playerInfo.UID},{(byte)1,s},
                };
            ConnectFunction.F.Deliver(4, dic);
            ConnectFunction.F.playerInfo.name = s;
        }
        else
            text.GetComponent<TextMeshPro>().text = "Too short";
    }

    public void S21checkevent(short returncode)
    {
        if (returncode == 0)
        {
            Application.LoadLevel("S22");
        }
        else if (returncode == 1)
        {
            text.GetComponent<TextMeshPro>().text = "has a same ID";
            ConnectFunction.F.playerInfo.name = "";
        }
    }
    public void OnDestroy()
    {
        ConnectFunction.F.S21checkevent -= S21checkevent;
    }

	// Use this for initialization
	void Start () {
        ConnectFunction.F.S21checkevent += S21checkevent;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

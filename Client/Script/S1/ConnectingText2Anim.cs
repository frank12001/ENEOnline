using UnityEngine;
using System.Collections;
using TMPro;

//目前沒使用到可刪掉
public class ConnectingText2Anim : MonoBehaviour {

    //middelText2的文字動畫
   float timer;
   TextMeshPro text;
	// Use this for initialization
	void Start () {
        timer = 0.0f;
        text = this.gameObject.GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
        timer = timer + Time.deltaTime;
        if (Mathf.Floor(timer) % 2 == 0)
        {
            text.text = "Connecting Server.";
        }
        else
        {
            text.text = "Connecting Server";
        }
	}
}

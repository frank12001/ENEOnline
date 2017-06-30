using UnityEngine;
using System.Collections;

public class GroupStop : MonoBehaviour {


    public void OnMouseDown()
    {
        SendMessageUpwards("GruopButtonOnClick", 2);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

﻿using UnityEngine;
using System.Collections;

public class GruopStart : MonoBehaviour {

    public void OnMouseDown()
    {
        SendMessageUpwards("GruopButtonOnClick", 1);          
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

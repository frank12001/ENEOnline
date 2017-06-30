using UnityEngine;
using System.Collections.Generic;

public class ComplexQ : ButtonBase
{
    public GameObject GroupMode;
    public GameObject SingleQButton;
    bool state = false; //用來控制，OnMouseUp的兩段功能
    void OnMouseUp()
    {
        if (!state)
        {
            GroupMode.SetActive(true);
            TriangleButton.GroupMode = true;
            this.SetText("   STOP");
            SingleQButton.SetActive(false);
            state = true;
        }
        else
        {
            GroupMode.SetActive(false);
            TriangleButton.GroupMode = false;
            this.SetText("  GROUP");
            SingleQButton.SetActive(true);
            state = false;
        }
    }
    
	// Use this for initialization
	void Start () {
        this.SetText("  GROUP");
        GroupMode.SetActive(false);
	}

}

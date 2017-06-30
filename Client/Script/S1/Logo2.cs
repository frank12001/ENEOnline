using UnityEngine;
using System.Collections;

public class Logo2 : MonoBehaviour {

    Animator anim;
    public void OnMouseDown()
    {
        switch (anim.GetInteger("state"))
        {
            case 0:
                SendMessageUpwards("LogoStateChange", 3);
                break;
            case 4:
                //進入下個場景
                break;
        }
    }
	// Use this for initialization
	void Start () {
        anim = this.GetComponentInParent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

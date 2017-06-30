using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {

    Animator anim;
    public void OnMouseDown()
    {
        switch (anim.GetInteger("state"))
        {
            case 0:
                    SendMessageUpwards("LogoStateChange", 1);
                    break;
            case 1:
                SendMessageUpwards("LogoStateChange", 2);
                    break;
            case 2:
                    SendMessageUpwards("LogoStateChange", 2);
                    break;
            case 3: 
                SendMessageUpwards("LogoStateChange", 2);
                    break;
            case 4:
                    anim.SetInteger("state", 1);
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

using UnityEngine;
using System.Collections;

public class ExitController : MonoBehaviour {

    public GameObject logo, bug;
    Animator animlogo, animbug;
    float timer;
    int logostate, bugstate;
    bool index;
	// Use this for initialization
	void Start () {
        timer = 0.0f;
        animlogo = logo.GetComponent<Animator>();
        animbug = bug.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!index && LogoAll.anim.GetInteger("state") == 5)
            index = true;
        if (index)
        {
            logostate = animlogo.GetInteger("state");
            bugstate = animbug.GetInteger("state");
            timer = timer + Time.deltaTime;
            switch (logostate)
            {
                case 5:
                   // animlogo.SetInteger("state", 6);
                    timer = 0.0f;
                    break;
                case 6:
                    if (timer >= 0.5 && bugstate != 0)
                        animbug.SetInteger("state", 0);
                    break;
                case 7:
                    if (timer >= 6.2 && bugstate != 1)
                        animbug.SetInteger("state", 1);
                    break;
                default:
                    break;
            }
            switch (bugstate)
            {
                case 0:
                    if (timer >= 2.0 && logostate != 7)
                        animlogo.SetInteger("state", 7);
                    break;
                default:
                    break;
            }
        }
	}
}

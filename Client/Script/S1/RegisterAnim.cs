using UnityEngine;
using System.Collections;
using TMPro;

public class RegisterAnim : MonoBehaviour {

    S1String stringpool;
    //middelText2的文字動畫
    float timer;
    TextMeshPro text;
    // Use this for initialization
    void Start()
    {
        stringpool = new S1String();
        timer = 0.0f;
        text = this.gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LogoAll.anim.GetCurrentAnimatorStateInfo(0).IsName("Register idle") || LogoAll.anim.GetCurrentAnimatorStateInfo(0).IsName("Login idle"))
        {
            timer = timer + Time.deltaTime;
            if (Mathf.Floor(timer) % 2 == 0 && !ConnectFunction.F.playerInfo.isConnect)
            {
                text.text = stringpool.connect1;
            }
            else if (ConnectFunction.F.playerInfo.isConnect)
            {
                text.text = "";  
            }
            else
            {
                text.text = stringpool.connect2;
            }
        }
        else
        {
            text.text = "";
        }
    }
}

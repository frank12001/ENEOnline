using UnityEngine;
using System.Collections;
using TMPro;

public class Registertext2Anim : MonoBehaviour {


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
        if (ConnectFunction.F.playerInfo.UID == null)
        {
            timer = timer + Time.deltaTime;
            if (Mathf.Floor(timer) % 2 == 0)
            {
                text.text = stringpool.register21;
            }
            else
            {
                text.text = stringpool.register22;
            }
        }
        else if (ConnectFunction.F.playerInfo.UID != null)
        {
                text.text = stringpool.register23;
        }
        
    }
}

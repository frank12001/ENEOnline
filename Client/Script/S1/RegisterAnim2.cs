using UnityEngine;
using System.Collections;
using TMPro;

public class RegisterAnim2 : MonoBehaviour {

    S1String stringpool;
    //middelText2的文字動畫
    float timer;
    TextMeshPro text;
	// Use this for initialization
	void Start () {
        stringpool = new S1String();
        timer = 0.0f;
        text = this.gameObject.GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!this.GetComponent<RegisterAnim>().enabled)
        {
            timer = timer + Time.deltaTime;
            if (Mathf.Floor(timer) % 2 == 0)
            {
                text.text = stringpool.animstring1;
            }
            else
            {
                text.text = stringpool.animstring2;
            }
        }
	}
}

using UnityEngine;
using System.Collections;
using TMPro;

public class chooseLogBase : MonoBehaviour {

    //Set----
    public void SetTopic(string topic)
    {
        TextMeshPro[] text = this.GetComponentsInChildren<TextMeshPro>();
       /* string result = "";
        int i = topic.Length / 20;
        for(int a =0 ;a<i;a++)
        {
            result = result + topic.Substring(a * 20, 20) + "\n";
        }
        result = result + topic.Substring(i * 20, topic.Length - (i * 20));
        text[2].text = result;
        * */
        text[2].text = topic;
    }
    public void SetTextButton1(string s)
    {
        TextMeshPro[] text = this.GetComponentsInChildren<TextMeshPro>();
        text[0].text = s;
    }
    public void SetTextButton2(string s)
    {
        TextMeshPro[] text = this.GetComponentsInChildren<TextMeshPro>();
        text[1].text = s;
    }
    //----

}

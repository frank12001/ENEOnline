using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class InputLogBase : MonoBehaviour {

    //Set----
    public void SetTopic(string topic)
    {
        TextMeshPro[] text = this.GetComponentsInChildren<TextMeshPro>();
        
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
    public void SetTextInput(string s)
    {
        InputField input = this.GetComponentInChildren<InputField>(); 
        input.text = s;
    }
    //----
    //Get---
    public string GetTextInput()
    {
        InputField input = this.GetComponentInChildren<InputField>();
         string s = input.text.ToString();
         return s;
    }
    //----
    //OnClick----
    public virtual void Button1Onclick()
    {
    }
    public virtual void Button2Onclick()
    {
    }
    //----
}

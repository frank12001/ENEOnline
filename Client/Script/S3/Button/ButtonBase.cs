using UnityEngine;
using System.Collections;
using TMPro;

public class ButtonBase : MonoBehaviour {

    TextMeshPro text;
    public void SetText(string s)
    {
        text = this.gameObject.GetComponentInChildren<TextMeshPro>();
        text.text = s;
    }
    public void SetTextColor(Color32 color)
    {
        text = this.gameObject.GetComponentInChildren<TextMeshPro>();
        text.faceColor = color;
    }
}

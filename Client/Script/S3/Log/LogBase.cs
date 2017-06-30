using UnityEngine;
using TMPro;

public class LogBase : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
	}
    public void SetColor(Color color)
    {
        TextMeshPro text = this.GetComponent<TextMeshPro>();
        text.color = color;
    }
    public void SetText(string s)
    {
        TextMeshPro text = this.GetComponent<TextMeshPro>();
        text.text = s;
    }
}

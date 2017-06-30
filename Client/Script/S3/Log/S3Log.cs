using UnityEngine;
using TMPro;

public class S3Log : MonoBehaviour {

    TextMeshPro mesh;
    byte a = 255;
    byte r, g, b;
    bool key;
		// Use this for initialization
	void Start () {
        mesh = this.gameObject.GetComponent<TextMeshPro>();
        r = mesh.faceColor.r;
        g = mesh.faceColor.g;
        b = mesh.faceColor.b;
        key = false;
	}
    public void FixedUpdate()
    {
        if (key)
        {
            a--;
            mesh.faceColor = new Color32(r, g, b, a);
            if (a <= 0)
            {
                SetText("");
                key = false;
            }
        }
        else
        {
            a = 255;
        }
    }
    public void SetColor(Color color)
    {
        TextMeshPro text = this.GetComponent<TextMeshPro>();
        text.color = color;
    }
    float timer;
    public void SetText(string s)
    {
        key = false;
        TextMeshPro text = this.GetComponent<TextMeshPro>();
        text.text = s;
        key = true;
    }
}

using UnityEngine;
using TMPro;
public class TextlogController : MonoBehaviour {


    TextMeshPro mesh;
    byte a = 255;
    byte r, g, b;
	// Use this for initialization
	void Start () {
        mesh = this.gameObject.GetComponent<TextMeshPro>();
        r = mesh.faceColor.r;
        g = mesh.faceColor.g;
        b = mesh.faceColor.b;
	}
    public void FixedUpdate()
    {
        a--;
        mesh.faceColor = new Color32(r, g, b, a);
        if (a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {

    }
   
}

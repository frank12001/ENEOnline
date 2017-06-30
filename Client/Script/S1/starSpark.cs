using UnityEngine;
using System.Collections;

public class starSpark : MonoBehaviour {

    MeshRenderer mesh;
    Color color;

    public enum ColorSelect : byte { Black=1,White,SiFiBlue };
    public ColorSelect MouseExit,MouseEnter;

    private Color32 sificolor = new Color32(92,248,255,255);

    public void OnMouseExit()
    {
        switch (MouseExit)
        {
            case ColorSelect.SiFiBlue:
                mesh.material.color = sificolor;
            break;
            case ColorSelect.White:
                mesh.material.color = Color.white;
            break;
            case ColorSelect.Black:
                mesh.material.color = Color.black;
            break;
            default:
                mesh.material.color = color;
            break;
        }
    }

    public void OnMouseEnter()
    {
        switch(MouseEnter)
        { 
            case ColorSelect.SiFiBlue:
                mesh.material.color = sificolor;
            break;
            case ColorSelect.White:
                mesh.material.color = Color.white;
            break;
            case ColorSelect.Black:
                mesh.material.color = Color.black;
            break;
            default:
                mesh.material.color = Color.white;
            break;
        }
    }

	// Use this for initialization
	void Start () {
        mesh = this.gameObject.GetComponent<MeshRenderer>();
        color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

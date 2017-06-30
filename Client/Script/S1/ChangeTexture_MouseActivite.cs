using UnityEngine;
using System.Collections;

public class ChangeTexture_MouseActivite : MonoBehaviour {
    


    MeshRenderer mesh;
    public Material MouseOver_Texture, MouseExit_Texture, MouseDown_Texture;
    // Use this for initialization
    void Start()
    {
        mesh = this.gameObject.GetComponent<MeshRenderer>();
    }

    void OnMouseExit()
    {
        mesh.material = MouseExit_Texture;
    }

    void OnMouseOver()
    {
        if(!Input.GetKey(KeyCode.Mouse0))
            mesh.material = MouseOver_Texture;
    }

    void OnMouseDown()
    {
        mesh.material = MouseDown_Texture;
        //--
        AudioController_S3.PlayerMusic(AudioController_S3.ButtonClick.onclick);
    }

    void OnMouseDrag()
    {
        mesh.material = MouseDown_Texture;
    }
}

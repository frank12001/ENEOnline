using UnityEngine;
using System.Collections;

public class textGetPoint : MonoBehaviour {

    Ray ray;
	// Use this for initialization
	void Start () {
        ray = new Ray(this.gameObject.transform.position, Vector3.up);
	}
	
	// Update is called once per frame
	void Update () {
        ray = new Ray(this.gameObject.transform.position, Vector3.up);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
        {
            Debug.Log("沒撞到");
            return;
        }
        Vector2 pixelUV = hit.textureCoord;
        float x = 128.0f, y = 128.0f;
        Debug.Log("X : " + (128-(int)(pixelUV.x * x)) + " Y : " + (int)(pixelUV.y * y) + " 撞到 : " + hit.collider.gameObject.name);
	}
}

using UnityEngine;
using System.Collections;

public class ButtonRemovefriend : MonoBehaviour {

    public void Update()
    {
        if (index == 1)
        {
            if (g == null)
            {
                this.gameObject.GetComponent<MeshCollider>().enabled = true;
                index = 0;
            }
        }
    }

    public GameObject RemovefriendsLog;
    GameObject g;
    int index = 0;
    public void OnMouseDown()
    {
        g = (GameObject)Instantiate(RemovefriendsLog);
        this.gameObject.GetComponent<MeshCollider>().enabled = false;
        index = 1;
    }
}

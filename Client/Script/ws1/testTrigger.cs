using UnityEngine;
using System.Collections;

public class testTrigger : MonoBehaviour {

    private GameObject g;
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("撞到了 ENTER");
        g = other.gameObject;
        if (other.gameObject.layer == 9)
            other.gameObject.layer = 8;
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("撞到了 Exit");
        if (other.gameObject.Equals(g))
        {
            other.gameObject.layer = 9;
        }
        g = null;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

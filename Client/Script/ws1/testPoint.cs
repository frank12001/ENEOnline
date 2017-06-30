using UnityEngine;
using System.Collections.Generic;

public class testPoint : MonoBehaviour {

    public GameObject target,output;
    Camera camera;

    List<int> list;
	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
      /*  list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        byte index =0;
        List<byte> d = new List<byte>();
        foreach (int i in list)
        {
            if (i == 2)
                d.Add(index);
                index++;
        }
        foreach (int i in d)
        {
            list.RemoveAt(i);
        }
        d.Clear();
        foreach (int i in list)
        {
            Debug.Log(i);
        }
        */

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Vector3 screenPos = camera.WorldToScreenPoint(target.transform.position);
            //Ray r = camera.ScreenPointToRay(screenPos);
            Instantiate(output, camera.ScreenPointToRay(camera.WorldToScreenPoint(target.transform.position)).GetPoint(0.20f), camera.gameObject.transform.rotation);
        }
	}
}

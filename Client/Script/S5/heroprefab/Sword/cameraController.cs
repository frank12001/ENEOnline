using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

    //使用時，要使用其他Script呼叫AssignCamera()才算抓到camera

    public GameObject camera;

    public float CameraShiftX, CameraShiftY, CameraShiftZ;
    //攝影機 以英雄為基準的位移量，因為要變成第三人稱，所以要有位移
    private Vector3 cameraShift;

	// Use this for initialization
	void Start () {
        cameraShift = new Vector3(CameraShiftX, CameraShiftY, CameraShiftZ);
        AssignCamera();
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    #region Function
    /// <summary>
    /// 當這個英雄是自己時呼叫的，將Main Camera只配給他。如果已經有指派過就沒有用(正常來說是要先用其他的Script判斷這個英雄要不要抓Camera在呼叫)
    /// </summary>
    public void AssignCamera()
    {
        if (camera == null)
        {
            //取得main camera
            camera = GameObject.Find("Main Camera");
            //先將camera和英雄的相對位置設好，在綁到hero身上
            camera.transform.position = this.gameObject.transform.position + cameraShift;
            //將camera 綁到 這個hero之下
            camera.transform.parent = transform;

            GameObject ui;
            //將跟隨相機的UI綁上去
            ui = GameObject.Find("UIObject");
            ui.transform.parent = camera.transform;
        }
    }
     #endregion
}

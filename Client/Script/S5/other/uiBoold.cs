using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class uiBoold : MonoBehaviour {

    public const float ExitTime = 4;

    private RectTransform[] rectA;
    private RectTransform rect;

    public Transform targetT;
    public Transform uiT;

    /// <summary>
    /// 改變血量條
    /// </summary>
    public float Blo{ get { return rect.sizeDelta.x; } 
                       set {
                           Vector2 v = new Vector2(value, 23.8f);
                           rect.sizeDelta = v;
                           } }

    public float time = 0.0f;

    private IPropertyBasic open;
    public bool b=false;
	// Use this for initialization
	void Start () {
        //uiT = gameObject.Get
        targetT = GameObject.Find("Main Camera").transform;
        rectA = this.gameObject.GetComponentsInChildren<RectTransform>();
        rect = rectA[2];
        rectA = null;
        open = this.gameObject.GetComponent<IPropertyBasic>();

        uiT.gameObject.SetActive(false);
	}
    
	
	// Update is called once per frame
	void Update () {
        if (b && !uiT.gameObject.activeSelf)
        {
            uiT.gameObject.SetActive(true);
        }
        uiT.rotation = Quaternion.Lerp(uiT.rotation, targetT.rotation, 1);
        if (time >= 4)
        {
            uiT.gameObject.SetActive(false);
            time = 0.0f;
            b = false;
        }
        else if (time < 4 && uiT.gameObject.activeSelf)
            time += Time.deltaTime;

        /* 測試用
        if (Input.GetKey(KeyCode.Space))
        {
            b = true;
        }
         * */
	}

}

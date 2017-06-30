using UnityEngine;
using UnityEngine.UI;

public class chat1 : MonoBehaviour {

    public void OnMouseUp()
    {
        this.gameObject.SendMessageUpwards("controllchat", 2);
        index = 0;
    }
    int index = 0;
    Vector3 pos1, pos2;
    public void OnMouseDrag()
    {
        //Debug.Log(Input.mousePosition);
        if (index == 0)
        {
            pos1 = Input.mousePosition;
            index = 1;
        }
        else if (index == 1)
        {
            Vector3 v = this.gameObject.transform.position;
            if ((v.x > -1.26816f && v.x < 1.731837f && v.y > -0.0400089f && v.y < 2.619995f) || CalculateDistance(Input.mousePosition.x, Input.mousePosition.y, 500.0f, 400.0f) <= CalculateDistance(pos1.x, pos1.y, 500.0f, 400.0f))
            {
                pos2 = Input.mousePosition;
                Debug.Log(CalculateDistance(Input.mousePosition.x, Input.mousePosition.y, v.x, v.y));
            }
            else if (!(v.x > -1.26816f && v.x < 1.731837f && v.y > -0.0400089f && v.y < 2.619995f) && CalculateDistance(Input.mousePosition.x, Input.mousePosition.y, 500.0f, 400.0f) <= CalculateDistance(pos1.x, pos1.y, 500.0f, 400.0f))
                pos2 = Input.mousePosition;
            else
                pos2 = pos1;
            //Debug.Log("x: " + v.x + "y: " + v.y);
            Vector3 pos3 = new Vector3(this.transform.position.x+((pos2.x-pos1.x)/200),this.transform.position.y+((pos2.y-pos1.y)/200),this.transform.position.z);
            this.transform.position = pos3;
            index = 0;
        }
    }
    /// <summary>
    /// 計算兩點間的距離
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public float CalculateDistance(float x1, float y1, float x2, float y2)
    {
        float distance = Mathf.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        return distance;
    }

    void OnMouseDown()
    {
        //this.gameObject.SendMessageUpwards("controllchat", 2);
    }
    Text[] text;
    MeshRenderer mesh;
    public void Awake()
    {
        mesh = this.gameObject.GetComponent<MeshRenderer>();
        color = mesh.material.color;
        text = this.gameObject.GetComponentsInChildren<Text>();
        //text[0] = 標題 
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //按右鍵關閉
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.Log(this.gameObject.name);
                if (hit.collider.gameObject.Equals(this.gameObject))
                {
                    this.gameObject.SendMessageUpwards("controllchat", 3);
                }
            }
        }

	}
    /// <summary>
    /// 回傳chat1 的 gameobject
    /// </summary>
    /// <returns></returns>
    public GameObject Getchat1()
    {
        return this.gameObject;
    }
    /// <summary>
    /// 設定chat1的標題
    /// </summary>
    /// <param name="name"></param>
    public void SetTopic(string name)
    {
        text[0].text = name;
    }
    /// <summary>
    /// gchat1閃爍開關
    /// </summary>
    /// <param name="start"></param>
    public void Spark(bool start)
    {
        //寫spark程式碼
        this.spark = start;
    }
    bool spark;
    Color color;
    float timer = 0.0f;
    void FixedUpdate()
    {
        if (spark)
        {
            timer = timer + Time.deltaTime;
            if ((int)timer==1)
            {
                mesh.material.color = Color.yellow; 
            }
            else if ((int)timer==2)
            {
                mesh.material.color = this.color;
                timer = 0.0f;
            }
        }
        else
        {
            timer = 0.0f;
            mesh.material.color = this.color;
        }
    }

}

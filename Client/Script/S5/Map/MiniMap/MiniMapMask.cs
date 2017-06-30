using UnityEngine;
using System.Collections.Generic;

public class MiniMapMask : MonoBehaviour {

    /// <summary>
    /// 取得這個Textture2D的 長寬。要有從Editor拖進去才有。
    /// </summary>
    private int originalHight, originalWight;
   /// <summary>
    /// 這個物件Material 上的第一張貼圖。要有從Editor拖進去才有
   /// </summary>
    Texture2D textture;

    /// <summary>
    /// 加入這裡面後，這個Script會自動計算亮點並讓它亮
    /// </summary>
    private Dictionary<string,Center> myTeammatesCenter;
    /// <summary>
    /// 加入myTeammatesCenter，讓它能被計算亮點
    /// </summary>
    /// <param name="name">這個Icon跟隨的人，的名子。也就是它的PlayerNumber</param>
    /// <param name="center">要製造得Center物件</param>
    /// <returns></returns>
    public bool AddTeammate(string name, Center center)
    {
        if (this.myTeammatesCenter.ContainsKey(name))
            return false;
        else
        {
            this.myTeammatesCenter.Add(name, center);
            return true;
        }
    }
    /// <summary>
    /// 看看有沒有在myTeammatesCenter
    /// </summary>
    /// <param name="keyTeammateName">Icon跟隨者的名子</param>
    /// <returns></returns>
    public bool ContainInTeamates(string keyTeammateName)
    {
        if (this.myTeammatesCenter.ContainsKey(keyTeammateName))
            return true;
        else
            return false;
    }

    /* //測試用Gameobjext物件
    public GameObject g1, g2;
     * */
	// Use this for initialization
	void Start () {

        myTeammatesCenter = new Dictionary<string, Center>();
        /* //測試用先新增幾個，要寫個方法讓startprepare 呼叫初始化
        myTeammatesCenter.Add(g1.name,new Center(g1));
        myTeammatesCenter.Add(g2.name,new Center(g2));
        */
        originalHight = this.GetComponent<MeshRenderer>().material.mainTexture.height;
        originalWight = this.GetComponent<MeshRenderer>().material.mainTexture.width;

        textture = new Texture2D(originalWight, originalHight);

       #region 第一次繪出亮點
        //第一次繪出亮點(其實也可以只在Onupdate中寫就好了)
        for (int i = 0; i < originalHight; i++)
        {
            for (int j = 0; j < originalWight; j++)
            {
                if (CheckthisPoint(10,i,j))
                {
                    Color color = new Color(0, 0, 0, 0);
                    textture.SetPixel(i, j, color);
                }
                else
                {
                    Color color = new Color(0, 0, 0, 0.8f);
                    textture.SetPixel(i, j, color);
                }
            }
        }

        textture.Apply();
        this.GetComponent<MeshRenderer>().material.mainTexture = textture;
       #endregion
	}

 
	
	// Update is called once per frame
	void Update ()
    {

        #region 計算要在哪裡繪出亮點
        foreach (KeyValuePair<string, Center> center in myTeammatesCenter)
        {
            Ray ray = new Ray(center.Value.Target.transform.position, Vector3.up);
            RaycastHit hit;
            Vector2 pixelUV;
            if (Physics.Raycast(ray, out hit))
                pixelUV = hit.textureCoord;
            else
                pixelUV = Vector2.zero;
            float x = 128.0f, y = 128.0f;
            //Debug.Log("X : " + (128-(int)(pixelUV.x * x)) + " Y : " + (int)(pixelUV.y * y));
            center.Value.Ox = 128 - (int)(pixelUV.x * x);
            center.Value.Oy = (int)(pixelUV.y * y);
        }
        #endregion

        #region 繪出亮點
        for (int i = 0; i < originalHight; i++)
        {
            for (int j = 0; j < originalWight; j++)
            {
                if (CheckthisPoint(10, i, j))
                {
                    Color color = new Color(0, 0, 0, 0);
                    textture.SetPixel(i, j, color);
                }
                else
                {
                    Color color = new Color(0, 0, 0, 0.8f);
                    textture.SetPixel(i, j, color);
                }
            }
        }
        textture.Apply();
        this.GetComponent<MeshRenderer>().material.mainTexture = textture;
        #endregion
    }


   /// <summary>
   /// 檢查這個點是否在myTeamatesCenter中任何一個圓中
   /// </summary>
    /// <param name="r">以myTeammatesCenter的圓心為圓的半徑</param>
   /// <param name="i">這個點的x</param>
   /// <param name="j">這個點的y</param>
   /// <returns></returns>
   public bool CheckthisPoint(int r,int i,int j)
    {
        foreach (KeyValuePair<string, Center> center in myTeammatesCenter)
        {
            if (InsideCircle(center.Value.Ox, center.Value.Oy, r, i, j))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 用圓心,半徑,點查一個點是否在裡面
    /// </summary>
    /// <param name="Ox">圓心x</param>
    /// <param name="Oy">圓心y</param>
    /// <param name="r">半徑</param>
    /// <param name="x">點x</param>
    /// <param name="y">點y</param>
    /// <returns></returns>
    private bool InsideCircle(int Ox, int Oy, int r, int x, int y)
    {
        if (((x - Ox) * (x - Ox)) + ((y - Oy) * (y - Oy)) <= r * r)
            return true;
        else
            return false;

    }
}


public class Center //用於儲存我的隊友要顯示的圓心
{
    /// <summary>
    /// 圓心Ox,Oy 預設(0,0)
    /// </summary>
    /// <param name="target">目標的GameObject物件，通常是放Icon</param>
    public Center(GameObject target)
    {
        this.Ox = 0;
        this.Oy = 0;
        this.Target = target;
    }
    public int Ox;
    public int Oy;
    public GameObject Target;
}

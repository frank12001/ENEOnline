using UnityEngine;
using System.Collections.Generic;

public class MeunPlane : MonoBehaviour {


    public GameObject item; //item預置物件
    public GameObject UpMG,BottonMG,LeftMG,RightMG; //四片葉子的位置
    List<GameObject> itemG = new List<GameObject>(); //存放已經實體出來的item
    List<int> list; //從放擁有英雄的hero id  
                    //hero id = 在 hero string 中的第幾的字母，從0開始 

    private string herostring; //這裡所使用的hero string ，不過還是要跟者 connectfunction.f.gameinfo.herostring做更改
    void Awake(){
        updateMyHeroList();
    }
	// Use this for initialization
	void Start () {
        
	}
    void OnEnable()
    {
        updateMyHeroList(); //以免有時 connectfunction.f.gameinfo.herostring 在 OnDisable 狀態下時有改變
        createbutton(list);
    }

    void OnDisable()
    {
        Destroybutton(itemG);
    }

    private void Destroybutton(List<GameObject> itemG)
    {
        foreach (GameObject g in itemG)
        {
            Destroy(g);
        }
        itemG.Clear();
    }

    /// <summary>
    /// 用暴力法創建按鈕，目前暴力到第5個
    /// </summary>
    /// <param name="list">這個list的長度</param>
    private void createbutton(List<int> list)
    {
        int index = 1;
        foreach (int i in list)
        {
            switch(index)
            { 
                case 1:
                 GameObject item1 = (GameObject)Instantiate(item, CalculatePos(0, 0), item.transform.rotation);
                 item1.name = i.ToString();
                 itemG.Add(item1);
                break;
                case 2:
                    GameObject item2 = (GameObject)Instantiate(item, CalculatePos(0, 2), item.transform.rotation);
                 item2.name = i.ToString();
                 itemG.Add(item2);
                break;
                case 3:
                GameObject item3 = (GameObject)Instantiate(item, CalculatePos(0, 4), item.transform.rotation);
                item3.name = i.ToString();
                itemG.Add(item3);
                break;
                case 4:
                GameObject item4 = (GameObject)Instantiate(item, CalculatePos(0, -2), item.transform.rotation);
                item4.name = i.ToString();
                itemG.Add(item4);
                break;
                case 5:
                GameObject item5 = (GameObject)Instantiate(item, CalculatePos(0, -4), item.transform.rotation);
                item5.name = i.ToString();
                itemG.Add(item5);
                break;
            
           }

            index++;
        }
    }

    /// <summary>
    /// 從ConnectFunction.F.gamingInfo.herostring取值，對這個class的hero string 做更改
    /// </summary>
    private void updateMyHeroList()
    {
        herostring = ConnectFunction.F.gamingInfo.herostring;
        list = HerostringTointList(herostring);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 將擁有(=1)的英雄，做成一個list<int>，List<int> 裡的值為擁有的英雄的id，就是在字串的第幾個的意思
    /// </summary>
    /// <param name="heroString"></param>
    /// <returns></returns>
    public List<int> HerostringTointList(string heroString)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < heroString.Length; i++)
        {
            string s = heroString.Substring(i,1);
            if (s == "1")
            {
                list.Add(i);
            }
        }
        return list;
    }
    /// <summary>
    /// x,y符合暴力表
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 CalculatePos(int x,int y)
    {
        Vector3 v = new Vector3();
        Vector3 h = new Vector3(0.0f,0.685f)/2,w = new Vector3(0.514f,0.0f)/2;
        v = UpMG.gameObject.transform.position;
        v = v + (x * h) + (y * w);

        return v;
        
    }
    

}

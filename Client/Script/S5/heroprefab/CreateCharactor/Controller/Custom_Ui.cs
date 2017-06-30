using UnityEngine;
using System.Collections.Generic;

public class Custom_Ui : MonoBehaviour {


    /// <summary>
    /// 用Tab打開的升等用 ui Plane 升等方式在SwordUI.cs裡。
    /// 在SwordUI.cs 裡自己抓取這個Plane。
    /// 要改"功能"或"輸入方式"的話要去 SwordUI.cs裡改。
    /// 這只是個顯示。
    /// </summary>
    public GameObject TabUiPlane;

    /// <summary>
    /// 這裡存放 這個角色的技能 cd剩餘時間 。Key 是 Skill_NAME 、Value 是 cd剩餘時間。
    /// Skill_NAME{ "CdMouse1", "CdMouse01", "CdF", "CdQ", "CdSpace", "CdE" }
    /// 如果這個角色沒有這個某個技能的話，用它的SKILL_NAME會搜不到
    /// </summary>
    public Dictionary<string,int> Skillcd = new Dictionary<string,int>();

    /// <summary>
    /// 存放"各部位現在得等級"。Parts為索引
    /// </summary>
    public Dictionary<Parts, byte> Level = new Dictionary<Parts, byte>();
    public enum Parts { head,hands,body,legs }; //各部位索引

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        /*foreach (KeyValuePair<string, int> item in Skillcd)
        {
            Debug.Log(item.Key + "  cd : " + item.Value);
        }
         * */
        /*foreach (KeyValuePair<Parts, byte> item in Level)
        {
            Debug.Log(item.Key + "  cd : " + item.Value);
        }
         * */
	}

    #region 當升等時執行一次的功能
    public void HeadLVUp()
    {
        
    }

    public void HandsLVUp()
    {
        
    }

    public void BodyLVUp()
    {
        
    }

    public void LegsLVUp()
    {

    }
    #endregion
}

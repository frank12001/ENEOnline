using UnityEngine;
using System.Collections;

public class AttackGenerator : MonoBehaviour
{

    #region 本程式碼注解
    //攻擊可分為兩部分 Generator(攻擊所發出的地方) / Gunsight 
    //Generator 就像槍口 / Gunsight就像準心
    //由Gunsight取得一點(瞄準點)後，和Generator所形成的向量就是攻擊方向 
    //Generator 不管是不是Sender 每個在場景中的英雄都有
    //是不是Sender 的差別在於 瞄準點 是從自己的Gunsight發Ray取得 還是從其他人那邊接收
    #endregion

    //Gunsight 參數             (Gunsight  中文翻譯為瞄準具)
    public GameObject Gunsight; //儲存 Gunsight預置物件
    private Vector3 GunsightShift;
    private GameObject gunsight; //儲存 Instantiate 後的Gameobject 物件

    //Generator 參數            (Generator 中文翻譯為發電機>>攻擊發電機==砲台)
    public GameObject attackGenerator;//儲存 attackGenerator預置物件 //砲台預製
    private Vector3 GeneratorShift;
    private GameObject generator;//儲存 Instantiate 後的Gameobject 物件 //這個才是真正的砲台

    //AttackObject 攻擊物件
    public GameObject AttackObject;


	// Use this for initialization
	void Start ()
    {
        #region Generator 設定
        GeneratorShift = new Vector3(-0.00274f,1.366072f,0.247761f);
        generator = (GameObject)Instantiate(attackGenerator, transform.position + GeneratorShift, transform.rotation);
        generator.name = "Attack generator";
        generator.transform.parent = transform;
        #endregion


        #region Gunsight 設定
        GunsightShift = new Vector3(0.0f, 0.0f, 1.0f);
        GameObject cameraC = GameObject.Find("Main Camera");
        gunsight = (GameObject)Instantiate(Gunsight, cameraC.transform.position + GunsightShift, Gunsight.transform.rotation);
        gunsight.name = "Gunsught";
        gunsight.transform.parent = cameraC.transform;

        #endregion

    }
	
	// Update is called once per frame
	void Update () {
        //暫時沒有要測試攻擊先注解掉
       /* if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GunsightShout(10.0f);
            //在這裡 實體出攻擊物件
            GameObject attack = Instantiate(AttackObject, generator.transform.position, generator.transform.rotation) as GameObject;
            attack.GetComponent<attackObject>().Damage = this.gameObject.GetComponent<FistProperty>().ATK;
            attack.GetComponent<Rigidbody>().AddRelativeForce(0.0f, 0.0f, 500.0f);

        }
        * */
    }

    #region Function
    //感覺還不是很完全，到軍中在想想   4/9留
    /// <summary>
    /// 呼叫後回傳從Gunsight 瞄到的點(回傳Vector3.zero的話就是沒撞到人)
    /// </summary>
    /// <param name="Maxdistance">最大射程(射線能到的最遠距離)</param>
    /// <returns></returns>
    public Vector3 GunsightShout(float maxDistance)
    {
        if (gunsight != null)
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            Ray ray = new Ray(gunsight.transform.position, fwd);
            if (Physics.Raycast(ray, out hit, maxDistance))
                return hit.point;               
        }
        return Vector3.zero;
        //ray的專用畫線 Debug Debug.DrawLine(ray.origin, ray.GetPoint(Maxdistance));
        
    }
    #endregion
}

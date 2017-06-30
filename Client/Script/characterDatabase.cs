using UnityEngine;
using System.Collections;

public class characterDatabase : MonoBehaviour {

    public GameObject[] CharactorDB = new GameObject[5];
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 實體化角色，暴力法，目前做到第五個
    /// </summary>
    /// <param name="characterID">角色ID</param>
    /// <param name="pos">要實體化的位置</param>
    public void InitializeCharacter(int characterID, Vector3 pos)
    {
        Instantiate(CharactorDB[characterID], pos, CharactorDB[characterID].transform.rotation);
    }
}

using UnityEngine;
using System.Collections;

public class Custom_Skill : MonoBehaviour {

    public bool Skill_Mouse1,Skill_Mouse01 , Skill_F, Skill_Q, Skill_Space, Skill_E;
	// Use this for initialization
	void Start () {
        //將可用的技能傳給 AnimController 
        this.GetComponent<SwordAnimController>().SetSkillAble(Skill_Mouse1,Skill_Mouse01 , Skill_F, Skill_Q, Skill_Space, Skill_E);
        this.GetComponent<SwordUI>().SkillAble = this.GetComponent<SwordAnimController>().GetSkillAble();
    }

    #region 當這個技能剛被使用時觸發
    public void Start_SkillMouse01()
    {
        //Debug.Log("M01M01");
    }
    public void Start_SkillMouse1()
    {
        //Debug.Log("M1M1");
    }
    public void Start_SkillF()
    {
        //Debug.Log("FF");
    }
    public void Start_SkillQ()
    {
        //Debug.Log("QQ");
    }
    public void Start_SkillSpace()
    {
        //Debug.Log("SPSP");
    }
    public void Start_SkillE()
    {
        //Debug.Log("EE");
    }
    #endregion

    #region 當這個技能處於cd狀態時不斷觸發
    public void KeepCDing_SkillMouse01()
    {
        //Debug.Log("M01M01");
    }
    public void KeepCDing_SkillMouse1()
    {
        //Debug.Log("M1M1");
    }
    public void KeepCDing_SkillF()
    {
        //Debug.Log("FF");
    }
    public void KeepCDing_SkillQ()
    {
        //Debug.Log("QQ");
    }
    public void KeepCDing_SkillSpace()
    {
        //Debug.Log("SPSP");
    }
    public void KeepCDing_SkillE()
    {
        //Debug.Log("EE");
    }
    #endregion

}

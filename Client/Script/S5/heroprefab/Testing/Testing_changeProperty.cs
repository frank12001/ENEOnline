#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// 偵對SwordProperty.cs 裡面的值做更改，達到測試最佳值的目的
/// </summary>
public class Testing_changeProperty : MonoBehaviour {

    SwordAnimController orginalCD;
    public bool NoController;
    SwordProperty property;
    public int ProgressBar_Max=100;
    /// <summary>
    /// 各部位能力值
    /// </summary>
    public float Blo, Atk, Def, Spe;

    /// <summary>
    /// 各技能減少的趴數。減少的cd時間公式 "原本固定的cd時間 * CDReduce = 新的cd時間"
    /// </summary>
    public float CDReduceMouse01, CDReduceMouse1, CDReduceF, CDReduceQ, CDReduceSpace, CDReduceE;
    public int CDReduce; //控制總合

    /// <summary>
    /// 開啟DeBug.log 檢察property 裡的數值
    /// </summary>
    public bool DebugOpen_Parts=true, DebugOpen_Reduces=true;

    /// <summary>
    /// 原始的CD時間
    /// </summary>
    public float CDOriginal_Mouse01, CDOriginal_Mouse1, CDOriginal_cdF, CDOriginal_cdQ, CDOriginal_cdSpace, CDOriginal_cdE;

	// Use this for initialization
	void Start () {
        property = this.GetComponent<SwordProperty>();
        property.Testing_BLO = 1000.0f;

        orginalCD = this.GetComponent<SwordAnimController>();
        if (orginalCD == null)
            NoController = true;
        if (!NoController)
        {
            DebugOpen_Parts = false;
            DebugOpen_Reduces = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        property.Testing_BLO = Blo;
        property.ATK = Atk;
        property.DEF = Def;
        property.SPE = Spe;

        property.CDReduceMouse01 = CDReduceMouse01 / CDReduce;
        property.CDReduceMouse1 = CDReduceMouse1 / CDReduce;
        property.CDReduceF = CDReduceF / CDReduce;
        property.CDReduceQ = CDReduceQ / CDReduce;
        property.CDReduceSpace = CDReduceSpace / CDReduce;
        property.CDReduceE = CDReduceE / CDReduce;

        property.OpenTesting_Parts = DebugOpen_Parts;
        property.OpenTesting_Reduces = DebugOpen_Reduces;

        if (!NoController)
        {
            orginalCD.cdMouse01 = CDOriginal_Mouse01;
            orginalCD.cdMouse1 = CDOriginal_Mouse1;
            orginalCD.cdF = CDOriginal_cdF;
            orginalCD.cdQ = CDOriginal_cdQ;
            orginalCD.cdSpace = CDOriginal_cdSpace;
            orginalCD.cdE = CDOriginal_cdE;
        }
	}
}
[CustomEditor(typeof(Testing_changeProperty))]
public class Testing_ChangingProperty : Editor
{
    protected static bool showHexTypes = false;
    protected static bool showCDTypes = false;
    protected static bool showSkillMouse1 = true, showSkillMouse01 = true, showSkillF = true, showSkillQ = true, showSkillSpace = true, showSkillE = true;

    string text = "使用此Script測完的數值，要手動回property或\nAnimController中修改。不然只能在編輯器模式下使用。";

    public override void OnInspectorGUI()
    {
        Testing_changeProperty mp = (Testing_changeProperty)target;

        #region 製作 Foldout 的 Style >> myFoldoutStyle
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 14;
        Color myStyleColor = Color.green;
        myFoldoutStyle.normal.textColor = myStyleColor;
        myFoldoutStyle.onNormal.textColor = myStyleColor;
        myFoldoutStyle.hover.textColor = myStyleColor;
        myFoldoutStyle.onHover.textColor = myStyleColor;
        myFoldoutStyle.focused.textColor = myStyleColor;
        myFoldoutStyle.onFocused.textColor = myStyleColor;
        myFoldoutStyle.active.textColor = myStyleColor;
        myFoldoutStyle.onActive.textColor = myStyleColor;
        #endregion

        #region 製作 Skill縮小 的 Style >> myskillStyle
        GUIStyle myskillStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 14;
        Color myStyleColors = Color.green;
        myFoldoutStyle.normal.textColor = myStyleColors;
        myFoldoutStyle.onNormal.textColor = myStyleColors;
        myFoldoutStyle.hover.textColor = myStyleColors;
        myFoldoutStyle.onHover.textColor = myStyleColors;
        myFoldoutStyle.focused.textColor = myStyleColors;
        myFoldoutStyle.onFocused.textColor = myStyleColors;
        myFoldoutStyle.active.textColor = myStyleColors;
        myFoldoutStyle.onActive.textColor = myStyleColors;
        #endregion

        EditorGUILayout.TextArea(text, GUILayout.Height(30));	

        EditorGUILayout.Space();
        showHexTypes = EditorGUILayout.Foldout(showHexTypes, "改變角色各部位能力值", myFoldoutStyle);
        if (showHexTypes)
        {
            mp.ProgressBar_Max = EditorGUILayout.IntSlider("下面能力值的最大值", (int)mp.ProgressBar_Max, 0, 1000000);

            mp.Blo = EditorGUILayout.IntSlider("BLO", (int)mp.Blo, 0, mp.ProgressBar_Max);
            ProgressBar(mp.Blo / mp.ProgressBar_Max, "BLO");

            mp.Atk = EditorGUILayout.IntSlider("ATK", (int)mp.Atk, 0, mp.ProgressBar_Max);
            ProgressBar(mp.Atk / mp.ProgressBar_Max, "ATK");

            mp.Def = EditorGUILayout.IntSlider("DEF", (int)mp.Def, 0, mp.ProgressBar_Max);
            ProgressBar(mp.Def / mp.ProgressBar_Max, "DEF");

            mp.Spe = EditorGUILayout.IntSlider("SPE", (int)mp.Spe, 0, mp.ProgressBar_Max);
            ProgressBar(mp.Spe / mp.ProgressBar_Max, "SPE");

            mp.DebugOpen_Parts = EditorGUILayout.Toggle("檢察在Property中的值", mp.DebugOpen_Parts);
        }
        EditorGUILayout.Space();
        if (!mp.NoController)
        {
            showCDTypes = EditorGUILayout.Foldout(showCDTypes, "改變技能CD", myFoldoutStyle);
            if (showCDTypes)
            {

                mp.CDReduce = EditorGUILayout.IntSlider("MAX", (int)mp.CDReduce, 0, 10000);
                EditorGUILayout.Space();

                showSkillMouse1 = EditorGUILayout.Foldout(showSkillMouse1, "", myskillStyle);
                if (showSkillMouse1)
                {
                    #region Mouse1
                    EditorGUILayout.LabelField("Mouse1現在的CD為", (mp.CDOriginal_Mouse1 * (mp.CDReduceMouse1 / mp.CDReduce)).ToString());
                    mp.CDOriginal_Mouse1 = EditorGUILayout.IntSlider("CDOriginal_Mouse1", (int)mp.CDOriginal_Mouse1, 0, 100);
                    mp.CDReduceMouse1 = EditorGUILayout.IntSlider("CDReduceMouse1", (int)mp.CDReduceMouse1, 0, mp.CDReduce);
                    EditorGUILayout.Space();
                    #endregion
                }
                showSkillMouse01 = EditorGUILayout.Foldout(showSkillMouse01, "", myskillStyle);
                if (showSkillMouse01)
                {
                    #region Mouse01
                    EditorGUILayout.LabelField("Mouse01現在的CD為", (mp.CDOriginal_Mouse01 * (mp.CDReduceMouse01 / mp.CDReduce)).ToString());
                    mp.CDOriginal_Mouse01 = EditorGUILayout.IntSlider("CDOriginal_Mouse01", (int)mp.CDOriginal_Mouse01, 0, 100);
                    mp.CDReduceMouse01 = EditorGUILayout.IntSlider("CDReduceMouse01", (int)mp.CDReduceMouse01, 0, mp.CDReduce);
                    EditorGUILayout.Space();
                    #endregion
                }
                showSkillF = EditorGUILayout.Foldout(showSkillF, "", myskillStyle);
                if (showSkillF)
                {
                    #region Skill_F
                    EditorGUILayout.LabelField("SkillF現在的CD為", (mp.CDOriginal_cdF * (mp.CDReduceF / mp.CDReduce)).ToString());
                    mp.CDOriginal_cdF = EditorGUILayout.IntSlider("CDOriginal_cdF", (int)mp.CDOriginal_cdF, 0, 100);
                    mp.CDReduceF = EditorGUILayout.IntSlider("CDReduceF", (int)mp.CDReduceF, 0, mp.CDReduce);
                    EditorGUILayout.Space();
                    #endregion
                }
                showSkillQ = EditorGUILayout.Foldout(showSkillQ, "", myskillStyle);
                if (showSkillQ)
                {
                    #region Skill_Q
                    EditorGUILayout.LabelField("SkillQ現在的CD為", (mp.CDOriginal_cdQ * (mp.CDReduceQ / mp.CDReduce)).ToString());
                    mp.CDOriginal_cdQ = EditorGUILayout.IntSlider("CDOriginal_cdQ", (int)mp.CDOriginal_cdQ, 0, 100);
                    mp.CDReduceQ = EditorGUILayout.IntSlider("CDReduceQ", (int)mp.CDReduceQ, 0, mp.CDReduce);
                    EditorGUILayout.Space();
                    #endregion
                }
                showSkillSpace = EditorGUILayout.Foldout(showSkillSpace, "", myskillStyle);
                if (showSkillSpace)
                {
                    #region Skill_Space
                    EditorGUILayout.LabelField("SkillSpace現在的CD為", (mp.CDOriginal_cdSpace * (mp.CDReduceSpace / mp.CDReduce)).ToString());
                    mp.CDOriginal_cdSpace = EditorGUILayout.IntSlider("CDOriginal_cdSpace", (int)mp.CDOriginal_cdSpace, 0, 100);
                    mp.CDReduceSpace = EditorGUILayout.IntSlider("CDReduceSpace", (int)mp.CDReduceSpace, 0, mp.CDReduce);
                    EditorGUILayout.Space();
                    #endregion
                }
                showSkillE = EditorGUILayout.Foldout(showSkillE, "", myskillStyle);
                if (showSkillE)
                {
                    #region Skill_E
                    EditorGUILayout.LabelField("SkillE現在的CD為", (mp.CDOriginal_cdE * (mp.CDReduceE / mp.CDReduce)).ToString());
                    mp.CDOriginal_cdE = EditorGUILayout.IntSlider("CDOriginal_cdE", (int)mp.CDOriginal_cdE, 0, 100);
                    mp.CDReduceE = EditorGUILayout.IntSlider("CDReduceE", (int)mp.CDReduceE, 0, mp.CDReduce);
                    EditorGUILayout.Space();
                    #endregion
                }

                EditorGUILayout.Space();
                mp.DebugOpen_Reduces = EditorGUILayout.Toggle("檢察在Property中的值", mp.DebugOpen_Reduces);
            }
        }

        //bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        //mp.gun = (GameObject)EditorGUILayout.ObjectField("Gun Object", mp.gun, typeof(GameObject), allowSceneObjects);
    }

    // Custom GUILayout progress bar.
    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
#endif
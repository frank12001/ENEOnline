using UnityEngine;
using System.Collections;

namespace Assets.Script.S4.Character
{
    public class Basic_Character : MonoBehaviour
    {
        //任何角色都要 使用/繼承 的腳本
        // Use this for initialization
        protected void Start()
        {
            animator_State = Animator.StringToHash("State");
            animator = gameObject.GetComponent<Animator>();
        }

        //------Character Information-------//
        public byte PlayerId;
        public byte OperatorId;
        [SerializeField]
        protected short hp;
        [SerializeField]
        protected short atk;
        public float moveSpeed;
        
        /// <summary>
        /// skill_1, 2, 3, 4 ,5
        /// </summary>
        public float[] skillCd=new float[5];

        //---Animator State
        [SerializeField]
        protected int animator_State;

        //CreateEffect 使用的參數
        protected Animator animator;
        private bool hasInstantiateEffect = false;

        /// <summary>
        /// 設定這為角色的各個資訊，在Render中使用
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="animation"></param>
        /// <param name="atk"></param>
        /// <param name="moveSpeed"></param>
        /// <param name="skillCd"></param>
        public void Set_CharacterInformation(short hp, short animation, short atk, float moveSpeed, float[] skillCd)
        {
            this.hp = hp;
            this.gameObject.GetComponent<Animator>().SetInteger(animator_State,animation);
            this.atk = atk;
            this.moveSpeed = moveSpeed;
            if(skillCd.Length==this.skillCd.Length)
            {
                for (byte i = 0; i < this.skillCd.Length; i++)
                {
                    this.skillCd[i] = skillCd[i];
                }
            }
        }
        /// <summary>
        /// 傳入在破撞器上的一個撞點，回傳此點在碰撞器中心線上的投影點 (方向向量 向上(0,1,0))
        /// </summary>
        /// <param name="point"></param>
        /// <returns>當沒此遊戲物件沒有CapsuleCollider時回傳Null</returns>
        public Vector3? GetProjectedPointsOnTheCapsuleColliderCenterLine(Vector3 point)
        {
            Vector3 ProjectedPoint = new Vector3();
            CapsuleCollider capsuleCollider = this.gameObject.GetComponent<CapsuleCollider>();
            if (capsuleCollider == null)
                return null;
            Vector3 center = capsuleCollider.bounds.center;
            #region 如果撞點 = 最高的點或最低的點 直接回傳
            float upestPoint = center.y + (capsuleCollider.height / 2);   //最高的點   
            float downestPoint = center.y - (capsuleCollider.height / 2); //最低的點
            if (point.x == center.x && point.z == center.z)
            {
                if (point.y == (center.y + upestPoint) || point.y == (center.y - downestPoint))   //如果撞點 = 最高的點或最低的點
                {
                    Debug.Log("直接回傳");
                    return point;  //直接回傳
                }
            }
            #endregion 
            ProjectedPoint.x = this.gameObject.transform.position.x;
            ProjectedPoint.y = point.y;
            ProjectedPoint.z = this.gameObject.transform.position.z;
            return ProjectedPoint;
        }

        /// <summary>
        /// 取得角色的中心(自己的位置是在角色底部)
        /// </summary>
        /// <returns>角色的中心 = 自己的位置+(Height/2)</returns>
        public Vector3? GetCenter()
        {
            CapsuleCollider capsuleCollider = this.gameObject.GetComponent<CapsuleCollider>();
            if (capsuleCollider == null)
                return null;
            else
            {
                Vector3 position = this.gameObject.transform.position;
                return new Vector3(position.x, position.y + (capsuleCollider.height / 2), position.z);
            }
        }

        /// <summary>
        /// 取得HP這個值
        /// </summary>
        /// <returns>HP</returns>
        public short GetHP()
        {
            return this.hp;
        }
        /// <summary>
        /// 取得動畫狀態(int) 
        /// </summary>
        /// <returns>animator 的 state (回傳 -1 代表角色上沒有Animator) </returns>
        public int GetAnimation()
        {
             return this.gameObject.GetComponent<Animator>().GetInteger(animator_State);
        }
        /// <summary>
        /// 藉由動畫狀態，創造特效
        /// </summary>
        /// <param name="effect_GameObject">特效遊戲物件</param>
        /// <param name="effect_position">特效的位置</param>
        /// <param name="stateName">動畫狀態的名子 (當這個角色執行這段動畫時，顯示特效)</param>
        /// <param name="animPercentage">在這段動畫跑到甚麼時候顯示特效?(百分比 0.0~1.0)</param>
        protected void CreateEffect(GameObject effect_GameObject,Vector3 effect_position,string stateName,float animPercentage)
        {
            if (animPercentage > 1 || animPercentage < 0)
                return;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) //attack0 連擊第一下
            {
                float runtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (runtime < animPercentage && runtime >= (animPercentage-0.1f) && !hasInstantiateEffect)
                {
                    if (effect_GameObject!=null)
                         Instantiate(effect_GameObject, effect_position, Quaternion.identity);
                    ExeWhenEffectInstantiate(stateName);
                    hasInstantiateEffect = true;
                }
                else if (runtime >= animPercentage && hasInstantiateEffect)
                    hasInstantiateEffect = false;
            }
        }
        /// <summary>
        /// 藉由動畫狀態，創造特效
        /// </summary>
        /// <param name="effect_GameObject">特效遊戲物件</param>
        /// <param name="effect_position">特效的位置</param>
        /// <param name="effect_rotation">特效的方向</param>
        /// <param name="stateName">動畫狀態的名子 (當這個角色執行這段動畫時，顯示特效)</param>
        /// <param name="animPercentage">在這段動畫跑到甚麼時候顯示特效?(百分比 0.0~1.0)</param>
        protected void CreateEffect(GameObject effect_GameObject, Vector3 effect_position,Quaternion effect_rotation, string stateName, float animPercentage)
        {
            if (animPercentage > 1 || animPercentage < 0)
                return;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) 
            {
                float runtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (runtime < animPercentage && runtime >= (animPercentage - 0.1f) && !hasInstantiateEffect)
                {
                    if (effect_GameObject != null)
                        Instantiate(effect_GameObject, effect_position, effect_rotation);
                    ExeWhenEffectInstantiate(stateName);
                    hasInstantiateEffect = true;
                }
                else if (runtime >= animPercentage && hasInstantiateEffect)
                    hasInstantiateEffect = false;
            }
        }
        /// <summary>
        /// 藉由動畫狀態，創造特效
        /// </summary>
        /// <param name="effect_GameObject">特效遊戲物件</param>
        /// <param name="effect_position">特效的位置</param>
        /// <param name="stateName">動畫狀態的名子 (當這個角色執行這段動畫時，顯示特效)</param>
        /// <param name="animPercentage_Max">在這段動畫得甚麼時候不要創造特效</param>
        /// <param name="animPercentageMin">在這段動畫得甚麼時候開始創造特效</param>
        protected void CreateEffect(GameObject effect_GameObject, Vector3 effect_position, string stateName, float animPercentage_Max, float animPercentageMin)
        {
            if (animPercentage_Max > 1 || animPercentageMin < 0)
                return;
            if (animPercentage_Max < animPercentageMin)
                return;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) 
            {
                float runtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (runtime < animPercentage_Max && runtime >= animPercentageMin && !hasInstantiateEffect)
                {
                    if (effect_GameObject != null)
                       Instantiate(effect_GameObject, effect_position, Quaternion.identity);
                    ExeWhenEffectInstantiate(stateName);
                    hasInstantiateEffect = true;                   
                }
                else if (runtime >= animPercentage_Max && hasInstantiateEffect)
                    hasInstantiateEffect = false;
            }
        }
        
        /// <summary>
        /// 藉由動畫狀態，創造特效。
        /// </summary>
        /// <param name="effect_GameObject">特效遊戲物件</param>
        /// <param name="effect_position">特效的位置</param>
        /// <param name="stateName">動畫狀態的名子 (當這個角色執行這段動畫時，顯示特效)</param>
        /// <param name="animPercentage">在這段動畫跑到甚麼時候顯示特效?(百分比 0.0~1.0)</param>
        /// <param name="followAttacker">此特效要不要跟者功擊者</param>
        protected void CreateEffect(GameObject effect_GameObject,Vector3 effect_position,string stateName,float animPercentage,bool followAttacker)
        {
            if (animPercentage > 1 || animPercentage < 0)
                return;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) //attack0 連擊第一下
            {
                float runtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (runtime < animPercentage && runtime >= (animPercentage-0.1f) && !hasInstantiateEffect)
                {
                    if (effect_GameObject != null)
                    {
                        GameObject g = Instantiate(effect_GameObject, effect_position, gameObject.transform.rotation) as GameObject;

                        if (followAttacker)
                            g.transform.parent = gameObject.transform;
                    }
                    ExeWhenEffectInstantiate(stateName);
                    hasInstantiateEffect = true;
                }
                else if (runtime >= animPercentage && hasInstantiateEffect)
                    hasInstantiateEffect = false;
            }
        }
        /// <summary>
        /// 在CreateEffect中，效果被創造時執行一次
        /// </summary>
        /// <param name="stateName">Animator State Name</param>
        protected virtual void ExeWhenEffectInstantiate(string stateName)
        {
 
        }
        /// <summary>
        /// 取得一點，到這位玩家碰撞器，最短路徑的碰撞點
        /// </summary>
        /// <param name="origin">起源點</param>
        /// <returns>碰撞點，沒撞到回傳 null</returns>
        protected Vector3? GetHitPoints(Vector3 origin)
        {
                float height = gameObject.GetComponent<CapsuleCollider>().height;
                Vector3 targetPoint = gameObject.transform.position;
                targetPoint.y += height / 2; //算出角色中心點
                Ray ray = new Ray(origin, targetPoint - origin);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    return hit.point;
                }
                else
                    return null;
        }
    }
}

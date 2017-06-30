using UnityEngine;
using System.Collections;

public class SenderMotion : MonoBehaviour {

    //此Class控制角色怎麼移動

    //控制此時能不能移動
    public bool movable = true;

    private SwordProperty property;
	// Use this for initialization
	void Start () {
        property = this.gameObject.GetComponent<SwordProperty>();
	}
    private Vector3 nextPos=new Vector3();
    private Vector3 nextPos2=new Vector3();
    public float forwardspeed = 0.2f;
    public float rightspeed = 0.2f;
    private float SPE;
	// Update is called once per frame
	void Update ()
    {
        this.SPE = property.SPE;
        #region move
        if (movable)
        {
            nextPos = Vector3.zero;
            nextPos2 = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                nextPos = Vector3.forward * 1 * (forwardspeed+(this.SPE/1000));
            if (Input.GetKey(KeyCode.S))
                nextPos2 = Vector3.forward * (-1) * (forwardspeed+(this.SPE/1000));
            if (Input.GetKey(KeyCode.A))
                nextPos2 = Vector3.left * 1 * (rightspeed+(this.SPE/1000));
            if (Input.GetKey(KeyCode.D))
                nextPos2 = Vector3.left * -1 * (rightspeed+(this.SPE/1000));
            this.transform.position = transform.TransformPoint(nextPos + nextPos2);
        }
        #endregion

    }
    public void StartMotion()
    {
        this.movable = true;
    }
    public void StopMotion()
    {
        this.movable = false;
    }
}

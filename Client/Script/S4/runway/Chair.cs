using UnityEngine;
using System.Collections;

public class Chair : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        CharactorOnChair = other.gameObject;
    }

    private GameObject g;
    public GameObject CharactorOnChair { get { return g; } 
            set {
                if(this.g!=null)
                {
                    Destroy(g.gameObject);
                    g = null;
                }
                g = value;
            } }
}

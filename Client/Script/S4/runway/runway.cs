using UnityEngine;
using System.Collections;

public class runway : MonoBehaviour {
    BoxCollider collider;
    string ModelName="";
    bool destroyModel = false;
    void Awake()
    {
        //collider = this.gameObject.GetComponent<BoxCollider>();
        //collider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        ModelName = other.gameObject.name;
        Debug.Log(other.gameObject.name);
        //collider.enabled = false;
        if (destroyModel)
            Destroy(other.gameObject);
    }
    public void openDestroyModel()
    {
        destroyModel = true;
    }
    public void closeDestroyModel()
    {
        destroyModel = false;
    }
    public string GetModelName()
    {
        return ModelName;     
    }
}

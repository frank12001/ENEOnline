using UnityEngine;
using UnityEngine.UI;

public class input : MonoBehaviour {
    public void Awake()
    {
        myinput = this.gameObject.GetComponentInChildren<InputField>();
    }

	// Use this for initialization
	void Start () {
        
	}
    InputField myinput;
    public string GetInputText()
    {
        return myinput.text;
    }
    public void Focus()
    {
        myinput.Select();
        myinput.ActivateInputField();
    }
}

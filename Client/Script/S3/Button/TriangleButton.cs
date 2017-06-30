using UnityEngine;
using System.Collections;

public class TriangleButton : MonoBehaviour {


    bool key;
    int index,buttonindex;
    public GameObject friendlist;
    public GameObject Button; //Button預置物件
    Buttonfriends[] ButtonCol; //實體Button 的存放空間
    public GameObject b1, b2,SingleQButton,GroupQButton;

    private void HideButton(bool hide)
    {
        if (hide)
        {
            b1.SetActive(false);
            b2.SetActive(false);
            SingleQButton.SetActive(false);
            GroupQButton.SetActive(false);
        }
        else
        {
            b1.SetActive(true);
            b2.SetActive(true);
            SingleQButton.SetActive(true);
            GroupQButton.SetActive(true);
        }
    }
    public void OnMouseDown()
    {
        AudioController_S3.PlayerMusic(AudioController_S3.ButtonClick.onclick);
        this.gameObject.GetComponent<MeshCollider>().enabled = false;
        key = true;
        ButtonSpark.Spark(false);
    }
    void Awake()
    {
        key = false;
        index = 0;
        buttonindex = 0;
    }
	// Use this for initialization
	void Start () {
        this.gameObject.transform.Rotate(new Vector3(0.0f,0.0f,180f));
	}
	
	// Update is called once per frame
	void Update () {
        if (key)
        {
            if (index == 0)
            {
                if (friendlist.transform.position.y >= -3.1f)
                {
                    if (buttonindex == 0)
                    {
                        HideButton(true);
                        DataManager.RemoveEntityButton(DataManager.ButtonCol);
                        buttonindex = 1;
                    }
                    friendlist.transform.position = new Vector3(friendlist.transform.position.x, friendlist.transform.position.y - 0.1f, friendlist.transform.position.z);
                }
                else
                {
                    buttonindex = 0;
                    index = 1;
                    key = false;
                    this.gameObject.GetComponent<MeshCollider>().enabled = true;
                    this.gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, -180f));
                }
            }
            else if (index == 1)
            {
                if (friendlist.transform.position.y <= -0.8f)
                {
                    friendlist.transform.position = new Vector3(friendlist.transform.position.x, friendlist.transform.position.y + 0.1f, friendlist.transform.position.z);
                }
                else
                {
                    HideButton(false);
                    DataManager.ButtonCol = DataManager.CreateButtonEntity(DataManager.datapool.FriendsList, DataManager.datapool.FriendsState, Button);
                    index = 0;
                    key = false;
                    this.gameObject.GetComponent<MeshCollider>().enabled = true;
                    this.gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, 180f));
                }
            }
        }
	}
}

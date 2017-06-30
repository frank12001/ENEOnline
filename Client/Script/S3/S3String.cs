using UnityEngine;
using System.Collections;

public class S3String{

    public S3String(string classname)
    {
        if (classname == "DataManager")
        {
              // text1 = "123";
        }
        else if (classname == "Addfriend")
        {
               InputLogtext1 = "Enter Name";
               InputLogtext2 = "Send";
               InputLogtext3 = "Back";
               Addfriendtext1 = "Has been friend";
               Addfriendtext2 = "Already exists";
               Addfriendtext3 = "Not exist";
               Addfriendtext4 = "Can not be yourself";
        }
        else if (classname == "Removefriend")
        {
            Removefriendtext1 = "Remove Friend";
            Removefriendtext2 = "Send";
            Removefriendtext3 = "Back";
            Removefriendtext4 = "Can not be yourself";
            Removefriendtext5 = "Do not have this friend";
        }
        else if (classname == "Community")
        {
            Communitytext1 = "Sent successfully,\nWaiting for reply";
            Communitytext2 = "The player does not exist";
            Communitytext3 = "The player is offline";
            Communitytext4 = " want to be \n your friend";
            Communitytext5 = "Are you Agree?";
            Communitytext6 = "Yes";
            Communitytext7 = "No";
            Communitytext8 = "refuse to be your friend";
            Communitytext9 = "Success Please refresh the page";
        }
        else if (classname == "content")
        {
            contenttext1 = "Other side";
        }
    }
    //DataManager 的字串
    // public string text1;
    //InputLog字串
    public string InputLogtext1;
    public string InputLogtext2;
    public string InputLogtext3;
    public string Addfriendtext1;
    public string Addfriendtext2;
    public string Addfriendtext3;
    public string Addfriendtext4;
    //Removefriend InputLog字串
    public string Removefriendtext1;
    public string Removefriendtext2;
    public string Removefriendtext3;
    public string Removefriendtext4;
    public string Removefriendtext5;
    //Community字串
    public string Communitytext1;
    public string Communitytext2;
    public string Communitytext3;
    public string Communitytext4;
    public string Communitytext5;
    public string Communitytext6;
    public string Communitytext7;
    public string Communitytext8;
    public string Communitytext9;
    //content 字串
    public string contenttext1;

}

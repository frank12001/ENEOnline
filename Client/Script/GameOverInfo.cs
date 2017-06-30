
static public class GameOverInfo{
    //存放我是從結束遊戲來的
    static public byte fromWhichScene = 3; //這個不清掉
    //在 離開 S6時將資料清掉釋放記憶體

    //自己有沒有贏
    static public bool IAmWin;
    //自己的playerNumber，用於計算獎勵。等於是在下面陣列自己的索引
    static public byte MyPlayerNumber;
    /// <summary>
    /// 參加者的名子(要照playernumber排)
    /// </summary>
    static public string[] NameParticipator = new string[10];
    /// <summary>
    /// 每個人的殺人數(要照playernumber排)
    /// </summary>
    static public byte[] KillNumber = new byte[10];
    /// <summary>
    /// 每個人的死亡數(要照playernumber排)
    /// </summary>
    static public byte[] DeathNumber = new byte[10];
    /// <summary>
    /// 剛剛同防人的uid (包跨自己的)
    /// </summary>
    static public int[] uidList = new int[10];
}

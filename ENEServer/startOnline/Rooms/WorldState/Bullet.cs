using System;

[Serializable]
public class Bullet
{

    /*
     * 創建一個圓形的子彈
     */

    public Bullet(byte unique_number, byte bulletMeshNumber, byte attackerNumber, byte state, float[] position, float[] direction, byte attacket_Direction, float speed, float damage, float radius, float lifetime_Max, float lifetime_Now)
    {
        this.Unique_Number = unique_number;
        this.BulletMeshNumber = bulletMeshNumber;
        this.AttackerNumber = attackerNumber;
        this.State = state;
        this.Position_now = (float[])position.Clone();
        this.Direction = direction;
        this.Attacket_Direction = attacket_Direction;
        this.Speed = speed;
        this.Damage = damage;
        this.Radius = radius;
        this.LifeTime_Max = lifetime_Max;
        this.LifeTime_Now = lifetime_Now;

    }
    public Bullet(byte unique_number, byte bulletMeshNumber, byte attackerNumber,bool fell ,byte state, float[] position, float[] direction, byte attacket_Direction, float speed, float damage, float radius, float lifetime_Max, float lifetime_Now)
    {
        this.Unique_Number = unique_number;
        this.BulletMeshNumber = bulletMeshNumber;
        this.AttackerNumber = attackerNumber;
        this.Fell = fell;
        this.State = state;
        this.Position_now = (float[])position.Clone();
        this.Direction = direction;
        this.Attacket_Direction = attacket_Direction;
        this.Speed = speed;
        this.Damage = damage;
        this.Radius = radius;
        this.LifeTime_Max = lifetime_Max;
        this.LifeTime_Now = lifetime_Now;

    }
    public byte Unique_Number;
    /// <summary>
    /// 定義子彈種類的編號
    /// </summary>
    public byte BulletMeshNumber;
    /// <summary>
    /// 功集者 Player id
    /// </summary>
    public byte AttackerNumber;
    /// <summary>
    /// 是不是倒地，預設為 False
    /// </summary>
    public bool Fell = false;
    /// <summary>
    /// 現在子彈的狀態 (1,2,3) (Birth , Mobile , Death)
    /// </summary>
    public byte State; //Birth -> Mobile -> Death
    /// <summary>
    /// 現在的子彈位置 = 圓心
    /// </summary>
    public float[] Position_now; // same of center
    /// <summary>
    /// 方向向量。(要讓他除以長度，變成單位向量在賦予)
    /// </summary>
    public float[] Direction;
    /// <summary>
    /// 攻擊者方向(32方位角)
    /// </summary>
    public byte Attacket_Direction;
    /// <summary>
    /// 速度 /sec
    /// </summary>
    public float Speed;
    /// <summary>
    /// 傷害
    /// </summary>
    public float Damage;
    /// <summary>
    /// 子彈的半徑
    /// </summary>
    public float Radius;
    /// <summary>
    /// 最大存活時間
    /// </summary>
    public float LifeTime_Max;
    /// <summary>
    /// 現在存活時間
    /// </summary>
    public float LifeTime_Now;
}


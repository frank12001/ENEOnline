using System;


[Serializable]
public struct Gamer
{
    public Gamer(byte number, byte heroid, float[] position, byte rotation, short hp, short atk, short animation_State, float moveSpeed,float collider_radius,float collider_height)
    {

        //---Set Basic Information
        this.number = number;
        this.heroid = heroid;
        //---Set Position Information
        this.Information_position.Position = new float[3];
        this.Information_position.Position[0] = position[0];
        this.Information_position.Position[1] = position[1];
        this.Information_position.Position[2] = position[2];

        this.Information_position.Rotation = rotation;

        //---Set Character Information
        this.Information_character.Hp = hp;
        this.Information_character.Atk = atk;
        this.Information_character.Animation = animation_State;
        this.Information_character.MoveSpeed = moveSpeed;
        this.Information_character.Collider_Radius = collider_radius;
        this.Information_character.Collider_Height = collider_height;

        this.Information_character.SkillCd = new float[5];
        this.Information_character.SkillCd[0] = (byte)0;
        this.Information_character.SkillCd[1] = (byte)0;
        this.Information_character.SkillCd[2] = (byte)0;
        this.Information_character.SkillCd[3] = (byte)0;
        this.Information_character.SkillCd[4] = (byte)0;
    }
    //------Basic Information------//
    public byte number; //自己的 player number
    public byte heroid; //自己的 hero id

    //------Position Information---//
    public Information_Position Information_position;

    //------Character Information-------//      
    public Information_Character Information_character;


}
[Serializable]
public struct Information_Position
{
    //------Position Information---//
    /// <summary>
    /// x,y,z
    /// </summary>
    public float[] Position;
    /// <summary>
    /// w,x,y,z
    /// </summary>
    public byte Rotation;
}
[Serializable]
public struct Information_Character
{
    public short Hp;
    public short Animation;
    public short Atk;
    public float MoveSpeed;
    /// <summary>
    /// skill_1, 2, 3, 4
    /// </summary>
    public float[] SkillCd;
    public float Collider_Radius;
    public float Collider_Height;
}


using System.IO;
using UnityEngine;
public class GameDataReader{

    public int Version {get;}
    BinaryReader reader;
    public GameDataReader(BinaryReader reader, int version){
        this.reader = reader;
        this.Version = version;
    }

    public float ReadFloat(){
        return this.reader.ReadSingle();
    }

    public int ReadInt(){
        return this.reader.ReadInt32();
    }

    public Quaternion ReadQuaternion(){
        Quaternion q;
        q.x = ReadFloat();
        q.y = ReadFloat();
        q.z = ReadFloat();
        q.w = ReadFloat();
        return q;
    }

    public Vector3 ReadVector3(){
        Vector3 v;
        v.x = ReadFloat();
        v.y = ReadFloat();
        v.z = ReadFloat();
        return v;
    }

    public Color ReadColor(){
        Color color;
        color.r = ReadFloat();
        color.g = ReadFloat();
        color.b = ReadFloat();
        color.a = ReadFloat();
        return color;
    }
}
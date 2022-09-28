using System.IO;
using UnityEngine;
public class GameDataReader{
    BinaryReader reader;
    public GameDataReader(BinaryReader reader){
        this.reader = reader;
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
}
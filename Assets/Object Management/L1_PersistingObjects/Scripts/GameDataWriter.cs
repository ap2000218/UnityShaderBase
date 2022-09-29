using System.IO;
using UnityEngine;
public class GameDataWriter{
    BinaryWriter writer;
    public GameDataWriter(BinaryWriter writer){
        this.writer = writer;
    }

    public void Write(int value){
        this.writer.Write(value);
    }

    public void Write(float value){
        this.writer.Write(value);
    }

    public void Write(Quaternion value){
        this.writer.Write(value.x);
        this.writer.Write(value.y);
        this.writer.Write(value.z);
        this.writer.Write(value.w);
    }

    public void Write(Vector3 value){
        this.writer.Write(value.x);
        this.writer.Write(value.y);
        this.writer.Write(value.z);
    }

    public void Write(Color color){
        this.writer.Write(color.r);
        this.writer.Write(color.g);
        this.writer.Write(color.b);
        this.writer.Write(color.a);
    }
}
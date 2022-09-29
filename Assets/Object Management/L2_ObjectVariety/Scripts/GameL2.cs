using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameL2 : PersistableObject
{
    [SerializeField]
    PersistentStorage storage;
    [SerializeField]
    KeyCode createKey = KeyCode.C;
    [SerializeField]
    KeyCode newGameKey = KeyCode.N;
    [SerializeField]
    KeyCode saveKey = KeyCode.S;
    [SerializeField]
    KeyCode loadKey = KeyCode.L;
    List<Shape> shapes;
    public ShapeFactory shapeFactory;
    const int saveVersion = 1;
    void Awake(){
        shapes = new List<Shape>();
    }

    void CreateShape(){
        Shape shape = shapeFactory.GetRandom();
        // Shape shape = shapeFactory.Get(0, 0);
        Transform t = shape.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shape.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        // shape.SetColor(Color.blue);
        shapes.Add(shape);
    }

    void BeginNewGame(){
        for (int i = 0; i < shapes.Count; i++)
        {
            Destroy(shapes[i].gameObject);
        }
        shapes.Clear();
    }

    public override void Save(GameDataWriter writer){
        // writer.Write(-saveVersion);
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        int count = version <=0 ? -version : reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape o = shapeFactory.Get(shapeId, materialId);
            o.Load(reader);
            shapes.Add(o);
        }
    }

    void Update(){
        if (Input.GetKeyDown(createKey))
        {
            CreateShape();
        }else if(Input.GetKeyDown(newGameKey)){
            BeginNewGame();
        }else if(Input.GetKeyDown(saveKey)){
            storage.Save(this, saveVersion);
        }else if(Input.GetKeyDown(loadKey)){
            BeginNewGame();
            storage.Load(this);
        }
    }
}

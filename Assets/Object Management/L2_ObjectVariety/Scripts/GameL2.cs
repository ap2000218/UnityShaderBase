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

    void Awake(){
        shapes = new List<Shape>();
    }

    void CreateShape(){
        Shape shape = shapeFactory.GetRandom();
        Transform t = shape.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
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
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            Shape o = shapeFactory.Get(0);
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
            storage.Save(this);
        }else if(Input.GetKeyDown(loadKey)){
            BeginNewGame();
            storage.Load(this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameL3 : PersistableObject
{
    [SerializeField]
    PersistentStorage storage;
    [SerializeField]
    KeyCode createKey = KeyCode.C;
    [SerializeField]
    KeyCode deleteKey = KeyCode.X;
    [SerializeField]
    KeyCode newGameKey = KeyCode.N;
    [SerializeField]
    KeyCode saveKey = KeyCode.S;
    [SerializeField]
    KeyCode loadKey = KeyCode.L;
    List<Shape> shapes;
    public ShapeFactory shapeFactory;
    const int saveVersion = 1;
    public float CreationSpeed {get;set;}
    public float DestructionSpeed {get; set;}
    float creationProgress;
    float destructionProgress;
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
            shapeFactory.Reclaim(shapes[i]);
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

    void DestroyShape(){
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            // Destroy(shapes[index].gameObject);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    void Update(){
        if (Input.GetKeyDown(createKey))
        {
            CreateShape();
        }else if(Input.GetKeyDown(deleteKey)){
            DestroyShape();
        }else if(Input.GetKeyDown(newGameKey)){
            BeginNewGame();
        }else if(Input.GetKeyDown(saveKey)){
            storage.Save(this, saveVersion);
        }else if(Input.GetKeyDown(loadKey)){
            BeginNewGame();
            storage.Load(this);
        }
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }
        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }

    }
}

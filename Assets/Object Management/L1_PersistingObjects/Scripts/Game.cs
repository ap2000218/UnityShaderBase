using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    [SerializeField]
    PersistableObject prefab;
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
    List<PersistableObject> objects;
    
    void Awake(){
        objects = new List<PersistableObject>();
    }

    void Update(){
        if (Input.GetKeyDown(createKey))
        {
            CreateObject();
        }else if(Input.GetKeyDown(newGameKey)){
            BeginNewGame();
        }else if(Input.GetKeyDown(saveKey)){
            storage.Save(this);
        }else if(Input.GetKeyDown(loadKey)){
            BeginNewGame();
            storage.Load(this);
        }
    }

    void CreateObject(){
        PersistableObject cube = Instantiate(prefab);
        Transform t = cube.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objects.Add(cube);
    }

    void BeginNewGame(){
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    public override void Save(GameDataWriter writer){
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }
}

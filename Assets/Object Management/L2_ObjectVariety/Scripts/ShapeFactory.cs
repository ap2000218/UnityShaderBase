using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    Shape[] prefabs;

    public Shape Get(int shapeId){
        Shape shape = Instantiate(prefabs[shapeId]);
        shape.ShapeId = shapeId;
        return shape;
    }

    public Shape GetRandom(){
        int shapeId = Random.Range(0, prefabs.Length);
        return Get(shapeId);
    }
}

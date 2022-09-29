using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField]
    Shape[] prefabs;
    [SerializeField]
    Material[] materials;

    public Shape Get(int shapeId = 0, int materialId = 0){
        Shape shape = Instantiate(prefabs[shapeId]);
        shape.ShapeId = shapeId;
        shape.SetMaterial(materials[materialId], materialId);
        return shape;
    }

    public Shape GetRandom(){
        int shapeId = Random.Range(0, prefabs.Length);
        int materialId = Random.Range(0, materials.Length);
        return Get(shapeId, materialId);
    }
}

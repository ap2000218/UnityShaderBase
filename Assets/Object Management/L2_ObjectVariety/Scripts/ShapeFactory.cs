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
    [SerializeField]
    bool recycle;
    List<Shape>[] pools;

    public Shape Get(int shapeId = 0, int materialId = 0){
        Shape shape = null;
        if (recycle)
        {
            if (pools == null)
            {
                CreatePools();       
            }
            List<Shape> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0)
            {
                shape = pool[lastIndex];
                shape.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
        }
        if (shape == null)
        {
            shape = Instantiate(prefabs[shapeId]);
            shape.ShapeId = shapeId;    
        }
        shape.SetMaterial(materials[materialId], materialId);
        return shape;
    }

    public Shape GetRandom(){
        int shapeId = Random.Range(0, prefabs.Length);
        int materialId = Random.Range(0, materials.Length);
        return Get(shapeId, materialId);
    }

    void CreatePools(){
        pools = new List<Shape>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<Shape>();
        }
    }

    public void Reclaim(Shape shapeToRecycle){
        if (recycle)
        {
            if (pools == null)
            {
                CreatePools();
            }
            pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }else{
            Destroy(shapeToRecycle.gameObject);
        }
    }
}

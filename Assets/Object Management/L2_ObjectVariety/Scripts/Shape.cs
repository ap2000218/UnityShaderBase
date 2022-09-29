using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    MeshRenderer meshRenderer;
    static int colorPropertyId = Shader.PropertyToID("_BaseColor");
    static MaterialPropertyBlock sharedPropertyBlock;
    int shapeId = int.MinValue;
    public int ShapeId {
        get{
            return shapeId;
        }
        set{
            if (shapeId == int.MinValue)
            {
                shapeId = value;   
            }
        }
    }
    public int MaterialId {get; private set;}
    Color color;

    void Awake(){
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material, int materialId){
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor(Color color){
        this.color = color;
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(sharedPropertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }
}

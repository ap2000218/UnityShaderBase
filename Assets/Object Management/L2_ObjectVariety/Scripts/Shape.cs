using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
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
}

using UnityEngine;

public class GraphSurface : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    [SerializeField, Range(10, 100)]
    int resolution = 10;
    [SerializeField]
    FunctionLibrary.FunctionName function = FunctionLibrary.FunctionName.Wave;
    Transform[] points;

    private void Awake() {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            var point =  Instantiate(pointPrefab);    
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    private void Update() {
        var time = Time.time;
        float step = 2f / resolution;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float v = 0.5f * step -1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step -1f;
            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }
}
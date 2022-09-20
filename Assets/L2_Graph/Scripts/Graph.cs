using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    [SerializeField, Range(10, 100)]
    int resolution = 10;
    Transform[] points;

    private void Awake() {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        Vector3 position = Vector3.zero;
        points = new Transform[resolution];
        for (int i = 0; i < resolution; i++)
        {
            var point =  Instantiate(pointPrefab);    
            position.x = ( i + 0.5f ) * step - 1f;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    private void Update() {
        var time = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var position = point.localPosition;
            position.y = Mathf.Sin( Mathf.PI * (position.x + time) );
            point.localPosition = position;
        }
    }
}
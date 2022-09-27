using UnityEngine;

public class GraphSurface : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    [SerializeField, Range(10, 100)]
    int resolution = 10;
    [SerializeField]
    FunctionLibrary.FunctionName function = FunctionLibrary.FunctionName.Wave;
    [SerializeField]
    float functionDuration = 1f, transitionDuration = 1f;
    Transform[] points;
    float duration = 0f;
    public enum TransitionMode{ Cycle, Random };
    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;
    bool transitioning = false;
    FunctionLibrary.FunctionName transitionFunction;

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
        duration += Time.deltaTime;
        if (!transitioning)
        {
            if (duration >= functionDuration)
            {
                duration -= functionDuration;
                transitioning = true;
                transitionFunction = function;
                PickNextFunction();
            }
        }
        if (transitioning)
        {
            UpdateFunctionTransition();
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }else{
            UpdateFunction();
        }
    }

    private void PickNextFunction(){
        if (transitionMode == TransitionMode.Cycle)
        {
            function = FunctionLibrary.GetFunctionName(function);
        }else{
            function = FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }
    }

    private void UpdateFunctionTransition(){
        FunctionLibrary.Function from = FunctionLibrary.GetFunction(transitionFunction);
        FunctionLibrary.Function to = FunctionLibrary.GetFunction(function);
        float progress = duration / transitionDuration;
        var time = Time.time;
        float step = 2f / resolution;
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
            points[i].localPosition = FunctionLibrary.Morph(u, v, time,  from, to, progress);
        }
    }

    private void UpdateFunction(){
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
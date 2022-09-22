using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;
    [SerializeField, Range(10, maxResolution)]
    int resolution = 200;
    [SerializeField]
    FunctionLibrary.FunctionName function = FunctionLibrary.FunctionName.Wave;
    [SerializeField]
    float functionDuration = 1f, transitionDuration = 1f;
    float duration = 0f;
    public enum TransitionMode{ Cycle, Random };
    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;
    [SerializeField]
    ComputeShader computeShader = default;
    [SerializeField]
    Material material = default;
    [SerializeField]
    Mesh mesh = default;
    bool transitioning = false;
    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionsBuffer;
    static readonly int positionId = Shader.PropertyToID("_Positions"),
    resolutionId = Shader.PropertyToID("_Resolution"),
    stepId = Shader.PropertyToID("_Step"),
    timeId = Shader.PropertyToID("_Time"),
    transitionProgressId = Shader.PropertyToID("_TransitionProgress");
    private void OnEnable() {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void OnDisable() {
        positionsBuffer.Release();
        positionsBuffer = null;
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
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }else{
            
        }
        UpdateFunctionOnGPU();
    }

    private void PickNextFunction(){
        if (transitionMode == TransitionMode.Cycle)
        {
            function = FunctionLibrary.GetFunctionName(function);
        }else{
            function = FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }
    }

    void UpdateFunctionOnGPU(){
        var kernelIndex = (int)function + (int)(transitioning ? transitionFunction : 0) * FunctionLibrary.FunctionCount;
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (transitioning)
        {
            computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f,duration / transitionDuration));
        }
        computeShader.SetBuffer(kernelIndex, positionId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);
        material.SetBuffer(positionId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

}

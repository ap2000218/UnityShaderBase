using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;

using static Unity.Mathematics.math;
using float3x3 = Unity.Mathematics.float3x3;
using quaternion = Unity.Mathematics.quaternion;
public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1,8)]
    int depth = 4;
    [SerializeField]
    Mesh mesh = default;
    [SerializeField]
    Material material = default;

    static float3[] directions = {
        up(), 
        right(), 
        left(), 
        forward(), 
        back()
    };
    static quaternion[] rotations = {
        quaternion.identity, 
        quaternion.RotateZ(-0.5f * PI),
        quaternion.RotateZ(0.5f * PI),
        quaternion.RotateX(0.5f * PI),
        quaternion.RotateX(-0.5f * PI)
    };

    struct FractalPart{
        public float3 direction, worldPosition;
        public quaternion rotation, worldRotation;
        public float spinAngle;
    }

    NativeArray<FractalPart>[] parts;
    NativeArray<float3x4>[] matrices;
    ComputeBuffer[] matricesBuffers;
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct UpdateFractalLevelJob:IJobFor {
        public float spinAngleDelta;
        public float scale;
        [ReadOnly]
        public NativeArray<FractalPart> parents;
        public NativeArray<FractalPart> parts;
        [WriteOnly]
        public NativeArray<float3x4> matrices;
        public void Execute(int i) {
            FractalPart parent = parents[i / 5];
            FractalPart part = parts[i];
            part.spinAngle += spinAngleDelta;
            part.worldRotation = mul(parent.worldRotation, mul(part.rotation, quaternion.RotateY(part.spinAngle)));
            part.worldPosition = parent.worldPosition + mul(parent.worldRotation, 1.5f * scale * part.direction);
            parts[i] = part;
            float3x3 r = float3x3(part.worldRotation) * scale;
            matrices[i] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
        }
    };

    static readonly int matricesId = Shader.PropertyToID("_Matrices");
    static MaterialPropertyBlock propertyBlock;
    private void OnEnable() {
        parts = new NativeArray<FractalPart>[depth];
        matrices = new NativeArray<float3x4>[depth];
        matricesBuffers = new ComputeBuffer[depth];
        int stride = 12 * 4;
        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
        {
            parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
            matrices[i] = new NativeArray<float3x4>(length, Allocator.Persistent);
            matricesBuffers[i] = new ComputeBuffer(length, stride);
        }
        parts[0][0] = CreatePart(0, 0);
        for (int li = 1; li < parts.Length; li++)
        {
            NativeArray<FractalPart> levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
            {
                for (int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(li, ci);
                }
            }
        }
        propertyBlock ??= new MaterialPropertyBlock();
    }

    private void OnDisable() {
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            matricesBuffers[i].Release();
            parts[i].Dispose();
            matrices[i].Dispose();
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;
    }

    private void OnValidate() {
        if (parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    FractalPart CreatePart(int levelIndex, int childIndex){
        return new FractalPart {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
        };
    }

    private void Update() {
        float spinAngleDelta = 0.125f * PI * Time.deltaTime;
        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = mul(transform.rotation, mul(rootPart.rotation, quaternion.RotateY(rootPart.spinAngle)));
        rootPart.worldPosition = transform.position;
        parts[0][0] = rootPart;
        float objectScale = transform.lossyScale.x;
        float3x3 r = float3x3(rootPart.worldRotation) * objectScale;
        matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.worldPosition);
        float scale = 1f;
        JobHandle jobHandle = default;
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            jobHandle = new UpdateFractalLevelJob{
                spinAngleDelta = spinAngleDelta,
                scale = scale,
                parents = parts[li - 1],
                parts = parts[li],
                matrices = matrices[li]
            }.ScheduleParallel(parts[li].Length, 5, jobHandle);
        }
        jobHandle.Complete();
        var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = matricesBuffers[i];
            buffer.SetData(matrices[i]);
            // material.SetBuffer(matricesId, buffer);
            propertyBlock.SetBuffer(matricesId, buffer);
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }
    
}

using System;
using UnityEngine;


[System.Serializable]
public class MeshGenerator
{
    enum MeshResolution : int 
    {
        Resolution_32x32 = 5, 
        Resolution_64x64 = 6, 
        Resolution_128x128 = 7, 
        Resolution_256x256 = 8
    }

    private const int _KERNEL_INDEX = 0;
    private const int _THREAD_GROUP_SIZE_X = 8;
    private const int _THREAD_GROUP_SIZE_Y = 8;
    private const int _THREAD_GROUP_SIZE_Z = 1;

    [SerializeField]
    private ComputeShader _compute;
    [SerializeField]
    private MeshResolution _maxResolution;
    [SerializeField]
    private float _scale;
    [SerializeField]
    private float _strength;

    private Vector3[] _vertices;
    private int[] _triangles;
    private Vector2[] _uvs;
    private int _resolution;

    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _triangleBuffer;
    private ComputeBuffer _uvBuffer;

    public void GenerateTile(RenderTexture heightMap, out Mesh[] tileLODs)
    {
        tileLODs = new Mesh[(int)_maxResolution];
        for(int i = (int)_maxResolution; i > 0; i++)
        {
            int res = Mathf.FloorToInt(Mathf.Pow(2.0f, i));
            ResizeData(res);
            SetBuffersData();

            SetBuffersToCompute();
            SetVariableDataToCompute();
            SetHeightMapToCompute(heightMap);
            DispatchCompute();
            ReadDataFromBuffers();

            GenerateMesh(ref tileLODs[i - 1]);
        }
    }

    private void GenerateMesh(ref Mesh mesh)
    {
        mesh = new Mesh();
        mesh.MarkDynamic();

        mesh.SetVertices(_vertices);
        mesh.SetTriangles(_triangles, 0);
        mesh.SetUVs(0, _uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    private void ResizeData(int resolution)
    {
        _resolution = resolution;

        int vertexSize = (resolution + 1) * (resolution + 1);
        int triangleSize = resolution * resolution * 2 * 3;
        _vertices = new Vector3[vertexSize];
        _triangles = new int[triangleSize];
        _uvs = new Vector2[vertexSize];

        _vertexBuffer = new ComputeBuffer(vertexSize, sizeof(float) * 3);
        _triangleBuffer = new ComputeBuffer(triangleSize, sizeof(int));
        _uvBuffer = new ComputeBuffer(vertexSize, sizeof(float) * 2);
    }

    private void SetBuffersData()
    {
        _vertexBuffer.SetData(_vertices);
        _triangleBuffer.SetData(_triangles);
        _uvBuffer.SetData(_uvs);
    }

    private void SetVariableDataToCompute()
    {
        _compute.SetInt("Resolution", _resolution);
        _compute.SetFloat("Scale", _scale);
        _compute.SetFloat("Strength", _strength);
    }

    private void SetHeightMapToCompute(RenderTexture heightMap)
    {
        _compute.SetTexture(_KERNEL_INDEX, "HeightMap", heightMap);
        _compute.SetInt("HeightMapResolution", heightMap.width);
    }

    private void SetBuffersToCompute()
    {
        _compute.SetBuffer(_KERNEL_INDEX, "Vertices", _vertexBuffer);
        _compute.SetBuffer(_KERNEL_INDEX, "Triangles", _triangleBuffer);
        _compute.SetBuffer(_KERNEL_INDEX, "UVs", _uvBuffer);
    }

    private void DispatchCompute()
    {
        _compute.Dispatch(_KERNEL_INDEX, (_resolution + 1) / _THREAD_GROUP_SIZE_X, (_resolution + 1) / _THREAD_GROUP_SIZE_Y, (_resolution + 1) / _THREAD_GROUP_SIZE_Z);
    }

    private void ReadDataFromBuffers()
    {
        _vertexBuffer.GetData(_vertices);
        _triangleBuffer.GetData(_triangles);
        _uvBuffer.GetData(_uvs);
    }

    public void DisposeBuffers()
    {
        _vertexBuffer.Dispose();
        _triangleBuffer.Dispose();
        _uvBuffer.Dispose();
    }
}
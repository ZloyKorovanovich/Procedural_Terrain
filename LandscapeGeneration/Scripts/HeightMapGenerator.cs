using UnityEngine;

[System.Serializable]
public class HeightMapGenerator
{
    enum HeightMapResolution : int 
    {
        Resolution_32x32 = 32, 
        Resolution_64x64 = 64, 
        Resolution_128x128 = 128, 
        Resolution_256x256 = 256, 
        Resolution_512x512 = 512, 
        Resolution_1024x1024 = 1024, 
        Resolution_2048x2048 = 2048
    }
    enum HeightMapDepth : int {Depth_16 = 16, Depth_32 = 32}

    private const int _KERNEL_INDEX = 0;
    private const int _THREAD_GROUP_SIZE_X = 32;
    private const int _THREAD_GROUP_SIZE_Y = 32;
    private const int _THREAD_GROUP_SIZE_Z = 1;

    [SerializeField]
    private ComputeShader _compute;
    [SerializeField]
    private HeightMapResolution _resolution = HeightMapResolution.Resolution_1024x1024;
    [SerializeField]
    private HeightMapDepth _depth = HeightMapDepth.Depth_16;
    [SerializeField]
    private int _octavesCount = 8;
    [SerializeField]
    private float _scale = 1.0f;
    [SerializeField, Range(1.0f, 2.0f)]
    private float _lacunarity = 1.3f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _persistance = 0.3f;

    private RenderTexture _heightMap;


    public void GenerateHeightMap(out RenderTexture heightMap, Vector2 shift)
    {
        ReleaseHeightMap();
        CreateRenderTexture();
        SetDataToCompute();
        SetMapDisplacementToCompute(shift);
        DispatchCompute();
        heightMap = _heightMap;
    }

    private void SetDataToCompute()
    {
        _compute.SetTexture(_KERNEL_INDEX, "HeightMap", _heightMap);
        _compute.SetInt("Resolution", (int)_resolution);
        _compute.SetInt("Octaves", _octavesCount);
        _compute.SetFloat("Scale", _scale);
        _compute.SetFloat("Lacunarity", _lacunarity);
        _compute.SetFloat("Persistance", _persistance);
    }
    private void SetMapDisplacementToCompute(Vector2 shift)
    {
        _compute.SetVector("Shift", shift);
    }

    private void DispatchCompute()
    {
        _compute.Dispatch(_KERNEL_INDEX, (int)_resolution / _THREAD_GROUP_SIZE_X, (int)_resolution / _THREAD_GROUP_SIZE_Y, _THREAD_GROUP_SIZE_Z);
    }

    private void CreateRenderTexture()
    {
        _heightMap = new RenderTexture((int)_resolution, (int)_resolution, (int)_depth);
        _heightMap.wrapMode = TextureWrapMode.Clamp;
        _heightMap.enableRandomWrite = true;

        _heightMap.Create();
    }

    private void ReleaseHeightMap()
    {
        if (_heightMap)
            _heightMap.Release();
    }
}

using UnityEngine;
using Unity.Jobs;

public class MeshGenerator : MonoBehaviour
{
    [Header("Tile Generation")]
    [SerializeField]
    private ComputeShader _tileDataGenerator;
    [SerializeField]
    private float _scale = 1f;

    private MeshFilter _meshFilter;

    private TileGenerator _tileGenerator;

    private void Awake()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _tileGenerator = new TileGenerator(_tileDataGenerator, _scale);
        gameObject.AddComponent<MeshRenderer>();
    }

    private void OnDestroy()
    {
        _tileGenerator.DisposeBuffers();
    }
}
public struct MeshData
{
    public int VertexCount => _verticesData.Length;
    public int TrianglesCount => _trianglesData.Length;
    public Vector3[] VerticesData => _verticesData;
    public int[] TrianglesData => _trianglesData;

    private Vector3[] _verticesData;
    private int[] _trianglesData;

    public MeshData(Vector3[] vertices, int[] triangles)
    {
        _verticesData = vertices;
        _trianglesData = triangles;
    }

    public void AddMesh(MeshData meshData)
    {
        meshData.VerticesData.CopyTo(_verticesData, _verticesData.Length);
        meshData.TrianglesData.CopyTo(_trianglesData, _trianglesData.Length);
    }
}
public class TileGenerator
{
    private const int _RESOLUTION = 15;

    private ComputeShader _meshGenerator;
    private float _scale;

    private Vector3[] _vertices = new Vector3[(_RESOLUTION + 1) * (_RESOLUTION + 1)];
    private int[] _triangles = new int[_RESOLUTION * _RESOLUTION * 2 * 3];

    private int _kernelIndex;
    private uint _threadGroupSize;

    private ComputeBuffer _verticesBuffer;
    private ComputeBuffer _trianglesBuffer;

    public TileGenerator(ComputeShader generator, float scale)
    {
        _scale = scale;
        _meshGenerator = generator;
    }

    public MeshData GetMeshData()
    {
        return new MeshData(_vertices, _triangles);
    }
    public Mesh GetMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        mesh.name = "Tile";
        return mesh;
    }
    public void ReadMeshData()
    {
        _verticesBuffer.GetData(_vertices);
        _trianglesBuffer.GetData(_triangles);
    }
    public void ComputeMesh()
    {
        _meshGenerator.SetBuffer(_kernelIndex, "Vertices", _verticesBuffer);
        _meshGenerator.SetBuffer(_kernelIndex, "Triangles", _trianglesBuffer);

        _meshGenerator.SetFloat("Scale", _scale);
        _meshGenerator.SetInt("Width", _RESOLUTION);

        _meshGenerator.Dispatch(_kernelIndex, _RESOLUTION + 1, 1, 1);
    }
    public void PrepareComputeData()
    {
        _verticesBuffer = new ComputeBuffer((_RESOLUTION + 1) * (_RESOLUTION + 1), sizeof(float) * 3);
        _trianglesBuffer = new ComputeBuffer(_RESOLUTION * _RESOLUTION * 2 * 3, sizeof(float));

        _verticesBuffer.SetData(_vertices);
        _trianglesBuffer.SetData(_triangles);

        _kernelIndex = _meshGenerator.FindKernel("GenerateQuad");
        _meshGenerator.GetKernelThreadGroupSizes(_kernelIndex, out _threadGroupSize, out _, out _);
    }
    public void DisposeBuffers()
    {
        _verticesBuffer.Dispose();
        _trianglesBuffer.Dispose();
    }
}

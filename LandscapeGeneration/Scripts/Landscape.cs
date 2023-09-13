using System.Collections.Generic;
using UnityEngine;


public class Landscape : MonoBehaviour
{
    [SerializeField]
    private HeightMapGenerator _heightMapGenerator;
    [SerializeField]
    private MeshGenerator _meshGenerator;
    

    [SerializeField]
    private Material _landscapeMaterial;
    [SerializeField]
    private Transform _spectator;

    private RenderTexture _heightMap;
    private GameObject _currentTile;
    private MeshFilter _tileFilter;
    private Mesh[] _tileMesh;

    private List<Chunk> _chunks = new List<Chunk>();

    private void GenerateChunk(Vector3 position)
    {
        _heightMapGenerator.GenerateHeightMap(out _heightMap, new Vector2(_spectator.position.x, _spectator.position.y));
        Mesh[] meshLODs;
        _meshGenerator.GenerateChunk(_heightMap, out meshLODs);

        GameObject chunkGameObject = new GameObject();
        chunkGameObject.name = "Chunk_" + _chunks.Count.ToString();

        Chunk chunk = chunkGameObject.AddComponent<Chunk>();
        chunk.Init(meshLODs, _landscapeMaterial);
        _chunks.Add(chunk);
    }

    private void Start()
    {
        GenerateChunk(Vector3.zero);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDisable()
    {
        _meshGenerator.DisposeBuffers();
    }
}

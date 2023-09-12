using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Landscape : MonoBehaviour
{
    [SerializeField]
    private HeightMapGenerator _heightMapGenerator;
    [SerializeField]
    private MeshGenerator _meshGenerator;

    [SerializeField]
    private float _strength;
    

    [SerializeField]
    private Material _landscapeMaterial;
    [SerializeField]
    private Transform _spectator;
    [SerializeField]
    private float _tileSpacing;

    private RenderTexture _heightMap;
    private GameObject _currentTile;
    private MeshFilter _tileFilter;
    private Mesh[] _tileMesh;

    private List<Chunk> _chunks;

    private void GenerateTile()
    {

    }

    private void GenerateChunk(Vector3 position)
    {
        _heightMapGenerator.GenerateHeightMap(out _heightMap, new Vector2(_spectator.position.x, _spectator.position.y));

        GameObject chunkGameObject = new GameObject();
        chunkGameObject.name = "Chunk" + _chunks.Count.ToString();

        Chunk chunk = chunkGameObject.AddComponent<Chunk>();

    }

    private void Start()
    {
        GenerateTile();
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDisable()
    {
        _meshGenerator.DisposeBuffers();
    }


    private void GenerateHeightMap()
    {
        _heightMapGenerator.GenerateHeightMap(out _heightMap, new Vector2(_spectator.position.x, _spectator.position.z));
    }
}

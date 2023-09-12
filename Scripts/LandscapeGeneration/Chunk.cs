using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLODs
{
    private MeshFilter _meshFilter;
    private Mesh[] _LODs;
    private int _currentLevel;

    public int CurrentLevel => _currentLevel;

    public ChunkLODs(Mesh[] LODs, MeshFilter meshFilter)
    {
        _meshFilter = meshFilter;
        _LODs = LODs;
        _currentLevel = 0;
    }

    public ChunkLODs(Mesh[] LODs, MeshFilter meshFilter, int level)
    {
        _meshFilter = meshFilter;
        _LODs = LODs;
        _currentLevel = level;
    }

    public void SetLOD(int level)
    {
        _currentLevel = level;
        SetCurrentLOD();
    }

    public void Increase()
    {
        _currentLevel++;
        SetCurrentLOD();
    }
    public void Increase(int amount)
    {
        _currentLevel += amount;
        SetCurrentLOD();
    }

    public void Decrease()
    {
        _currentLevel--;
        SetCurrentLOD();
    }

    public void Decrease(int amount)
    {
        _currentLevel -= amount;
        SetCurrentLOD();
    }

    private void SetCurrentLOD()
    {
        Mathf.Clamp(_currentLevel, 0, _LODs.Length - 1);
        _meshFilter.mesh = _LODs[_currentLevel];
    }
}

public class Chunk : MonoBehaviour
{
    private ChunkLODs _LODs;
    public void SetParametres(MeshFilter meshFilter, Mesh[] LODs, MeshRenderer meshRenderer, Landscape parent)
    {
        _LODs = new ChunkLODs(LODs, meshFilter);
    }

    
}

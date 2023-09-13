using UnityEngine;

public class ChunkLODs : MonoBehaviour
{
    private Mesh[] _LODs;
    private MeshFilter _meshFilter;
    private int _currentLevel;

    public int CurrentLevel => _currentLevel;
    public void Init(Mesh[] LODs)
    {
        _LODs = LODs;
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        SetLOD(0);
    }
    public void Init(Mesh[] LODs, int level)
    {
        _LODs = LODs;
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        SetLOD(level);
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
        _currentLevel = Mathf.Clamp(_currentLevel, 0, _LODs.Length - 1);
        _meshFilter.mesh = _LODs[_currentLevel];
    }
}

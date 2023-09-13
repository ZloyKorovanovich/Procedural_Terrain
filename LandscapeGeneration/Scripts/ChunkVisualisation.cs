using UnityEngine;

public class ChunkVisualisation : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Material _material;

    public void Init(Material material)
    {
        _material = material;
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        _meshRenderer.material = _material;
    }
}

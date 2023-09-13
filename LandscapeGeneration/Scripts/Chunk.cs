using UnityEngine;

public class Chunk : MonoBehaviour
{
    private ChunkLODs _LODs;
    private ChunkVisualisation _Visualisation;

    public void Init(Mesh[] LODs, Material material)
    {
        _LODs = gameObject.AddComponent<ChunkLODs>();
        _LODs.Init(LODs);

        _Visualisation = gameObject.AddComponent<ChunkVisualisation>();
        _Visualisation.Init(material);
    }
}

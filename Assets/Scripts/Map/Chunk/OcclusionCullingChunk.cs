using UnityEngine;

public class OcclusionCullingChunk : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            Chunk chunk = other.GetComponent<Chunk>();
            chunk.EnabledCombinedMesh();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            Chunk chunk = other.GetComponent<Chunk>();
            chunk.DisableCombinedMesh();
        }
    }
}

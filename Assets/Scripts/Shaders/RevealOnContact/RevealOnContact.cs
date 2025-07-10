using UnityEngine;

public class RevealOnContact : MonoBehaviour
{
    Material revealMaterial;
    public float revealRadius = 1f;
    public float falloff = 0.2f;
    public MeshRenderer meshRenderer;
    void Start()
    {
        revealMaterial = meshRenderer.material;
        revealMaterial.SetFloat("_RevealRadius", revealRadius);
        revealMaterial.SetVector("_RevealPosition", Vector3.one * 9999);
        revealMaterial.SetFloat("_Falloff", falloff);
    }
    void OnCollisionStay(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Vector3 pos = other.transform.position;
            revealMaterial.SetVector("_RevealPosition", pos);
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            revealMaterial.SetVector("_RevealPosition", Vector3.one * 9999);
        }
    }
}
using UnityEngine;
using UnityEngine.UIElements;

public class RevealOnContact : MonoBehaviour
{
    public Vector3 offset;
    public Transform revealObject;
    void OnCollisionStay(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            revealObject.gameObject.SetActive(true);
            revealObject.position = other.contacts[0].point + offset;
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            revealObject.gameObject.SetActive(false);
            revealObject.localPosition = Vector3.zero;
        }
    }
}
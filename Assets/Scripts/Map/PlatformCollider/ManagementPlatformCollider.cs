using UnityEngine;

public class ManagementPlatformCollider : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) other.transform.SetParent(transform);
    }
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) other.transform.SetParent(null);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) other.transform.SetParent(transform);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) other.transform.SetParent(null);
    }
}

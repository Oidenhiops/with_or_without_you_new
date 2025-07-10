using System.Collections;
using UnityEngine;

public class ManagementPushEffect : MonoBehaviour, ManagementStatusEffect.IStatusEffect
{
    public float pushForce = 5;
    public IEnumerator ApplyStatusEffect(GameObject objectMakeEffect, GameObject objectToMakeEffect)
    {
        Vector3 direction = (objectToMakeEffect.transform.position - objectMakeEffect.transform.position).normalized;
        Rigidbody rb = objectToMakeEffect.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * pushForce * rb.mass, ForceMode.Impulse);
        }
        yield return null;
    }
}

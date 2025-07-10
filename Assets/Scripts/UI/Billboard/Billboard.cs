using UnityEngine;

public class Billboard : MonoBehaviour
{
    public GameObject objectToBillboard;
    private void FixedUpdate()
    {
        if (Camera.main != null)
        {
            objectToBillboard.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
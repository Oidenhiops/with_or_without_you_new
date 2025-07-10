using UnityEngine;

public class GrassTrack : MonoBehaviour
{
    Vector3 trackerPos;
    Material grassMat;
    [SerializeField] Renderer grassRenderer;
    float size = 0.65f;
    Vector3 offset = new Vector3(0, 0.32f, 0);
    float rayDistance = 0.25f;
    [SerializeField] LayerMask layerMask;
    float time = 0;
    float speed = 0.1f;
    void Start()
    {
        grassMat = grassRenderer.material;
    }
    void FixedUpdate()
    {
        RaycastHit[] objects = Physics.BoxCastAll(transform.position + offset, Vector3.one * size, Vector3.up, Quaternion.identity, rayDistance, layerMask);

        if (objects.Length > 0)
        {
            time = 0;
            trackerPos = GetMidpoint(objects);
            grassMat.SetVector("_TrakerPosition", trackerPos);
        }
        else if (grassMat.GetVector("_TrakerPosition") != Vector4.zero)
        {
            time += Time.deltaTime * speed;
            trackerPos = Vector3.Lerp(grassMat.GetVector("_TrakerPosition"), Vector3.zero, time * Time.deltaTime);
            grassMat.SetVector("_TrakerPosition", trackerPos);
        }
    }
    Vector3 GetMidpoint(RaycastHit[] transforms)
    {
        Vector3 sum = Vector3.zero;
        foreach (var t in transforms)
        {
            sum += t.transform.position;
        }
        return sum / transforms.Length;
    }
}

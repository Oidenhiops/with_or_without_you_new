using UnityEngine;

public class ManagementLigthsObjects : MonoBehaviour
{
    public Light ligth;
    public float intensityOffset = -5f;
    public float intensity = 10;
    public float speed = 2f;

    private float time;

    void Start()
    {
        ligth.intensity = intensity - 5;
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        ligth.intensity = Mathf.Lerp(intensity - intensityOffset, intensity, Mathf.PingPong(time, 1));
    }
}

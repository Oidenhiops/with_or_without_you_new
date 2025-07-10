using Unity.Cinemachine;
using UnityEngine;

public class ManagementPlayerCamera : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] CinemachineOrbitalFollow vcam;
    [SerializeField] float baseSpeed = 0.01f;
    [SerializeField] float currentSpeed;
    public void Start()
    {
        GameManager.Instance.OnDeviceChanged += ChangeSpeedCamera;
    }
    public void OnDestroy()
    {
        GameManager.Instance.OnDeviceChanged -= ChangeSpeedCamera;
    }
    void ChangeSpeedCamera(GameManager.TypeDevice typeDevice)
    {
        currentSpeed = typeDevice == GameManager.TypeDevice.PC ? baseSpeed : baseSpeed * 40;
    }
    public void CamDirection(out Vector3 camForward, out Vector3 camRight)
    {
        camForward = Camera.main.transform.forward;
        camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }
}

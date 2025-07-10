using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MenuHelper : MonoBehaviour
{
    public Button playButton;
    public CinemachineCamera cinemachineCamera;
    public void SelectButton()
    {
        playButton.Select();
    }
    public void PrioritiseCharacterCamera()
    {
        cinemachineCamera.Priority = 10;
    }
    public void UnprioritizedCharacterCamera()
    {
        cinemachineCamera.Priority = 0;
    }
}

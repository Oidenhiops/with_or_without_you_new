using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MenuHelper : MonoBehaviour
{
    public Button lastButtonSelected;
    public CinemachineCamera cinemachineCamera;
    public void SelectButton()
    {
        lastButtonSelected.Select();
    }
    public void PrioritiseCharacterCamera()
    {
        cinemachineCamera.Priority = 10;
    }
    public void UnprioritizedCharacterCamera()
    {
        cinemachineCamera.Priority = 0;
    }
    public void ChangeLastButtonSelected(Button button)
    {
        lastButtonSelected = button;
    }
}

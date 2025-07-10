using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsHelper : MonoBehaviour
{
    public Button creditsButton;
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Invoke("ActiveButton", 0.25f);
    }
    void ActiveButton()
    {
        creditsButton.Select();
    }
}

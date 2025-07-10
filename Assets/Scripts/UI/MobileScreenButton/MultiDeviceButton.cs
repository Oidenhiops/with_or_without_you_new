using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
public class MultiDeviceButton : MonoBehaviour
{
    public SerializedDictionary<GameManager.TypeDevice, GameObject> buttons;
    void Start()
    {
        ValidateScreenButton(GameManager.Instance._currentDevice);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDeviceChanged += ValidateScreenButton;   
        }      
    }
    void OnDestroy()
    {
        GameManager.Instance.OnDeviceChanged -= ValidateScreenButton;
    }
    void ValidateScreenButton(GameManager.TypeDevice device)
    {
        foreach (var button in buttons)
        {
            button.Value.SetActive(button.Key == device);
        }
    }
}

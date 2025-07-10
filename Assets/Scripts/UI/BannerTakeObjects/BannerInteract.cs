using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BannerInteract : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public Image spriteObject;
    public TMP_Text textObject;
    public GameObject takeButton;
    public ManagementCharacterObjects managementCharacterObjects;
    public ManagementCharacterInteract managementCharacterInteract;
    public GameObject objectForTake;
    public ManagementLanguage managementLanguage;
    public CanvasGroup canvasGroup;
    public OnObjectSelect onObjectSelect;
    public void TakeObject()
    {
        if (objectForTake.TryGetComponent<ManagementInteract>(out ManagementInteract managementInteract))
        {
            if (managementInteract.typeInteract == ManagementInteract.TypeInteract.Item)
            {
                managementCharacterObjects.TakeObject(objectForTake);
            }
            else if (managementInteract.typeInteract == ManagementInteract.TypeInteract.Object)
            {
                objectForTake.GetComponent<ManagementInteract.IObjectInteract>().Interact(managementCharacterObjects.character);
            }
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (!managementCharacterInteract.isRefreshInteracts)
        {
            managementCharacterInteract.currentInteractIndex = transform.GetSiblingIndex();
        }
        managementCharacterInteract.currentInteract = objectForTake;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!managementCharacterInteract.isRefreshInteracts)
        {
            managementCharacterInteract.currentInteractIndex = transform.GetSiblingIndex();
        }
        managementCharacterInteract.currentInteract = objectForTake;
    }
    public void EnableButton()
    {
        takeButton.SetActive(true);
    }
    public void DisableButton()
    {
        takeButton.SetActive(false);
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownSelected : MonoBehaviour, ISubmitHandler, IPointerDownHandler
{
    public TMP_Dropdown dropdown;
    public bool isInit;
    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        print($"Selected: {gameObject.name} whit {dropdown.value} value");
        if (isInit)
        {
            isInit = false;
        }
        _= ScrollTo();
    }
    void OnDropdownValueChanged(int index)
    {
        isInit = false;
    }
    public async Awaitable ScrollTo()
    {
        try
        {
            OnObjectSelect component = transform.GetChild(transform.childCount - 1).GetComponentInChildren<OnObjectSelect>();
            await Awaitable.NextFrameAsync();
            if (component)
            {
                component.ForceScroll(component.dropdown.value);
                isInit = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    public void OnSubmit(BaseEventData eventData)
    {
        print($"Selected: {gameObject.name} whit {dropdown.value} value");
        if (isInit)
        {
            isInit = false;
        }
        _= ScrollTo();
    }
}

using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Portal : MonoBehaviour, ManagementInteract.IObjectInteract
{
    public ManagementInteract managementInteract;
    public Action<Character> OnCharacterEnter;
    public Portal otherPortal;
    public bool needDissapearAfterEnter = false;
    public void Start()
    {
        OnCharacterEnter += CharacterEnterInPortal;
    }
    public void CharacterEnterInPortal(Character character)
    {
        character.transform.position = transform.position;
        if (needDissapearAfterEnter)
        {
            managementInteract.canInteract = false;
        }
    }
    public void Interact(Character character)
    {
        otherPortal.OnCharacterEnter?.Invoke(character);
    }
}
public enum TypePortal
{
    PortalA = 0,
    PortalB = 1
}

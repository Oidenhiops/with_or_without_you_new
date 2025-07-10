using UnityEngine;

public class ManagementInteract : MonoBehaviour
{
    public int IDText = 0;
    public TypeInteract typeInteract;
    public bool canInteract = false;
    public void Interact(Character character)
    {
        if (typeInteract == TypeInteract.Item)
        {
            PickUp(character);
        }
        else if (typeInteract == TypeInteract.Object){
            GetComponent<IObjectInteract>().Interact(character);
        }
    }
    public void PickUp(Character character)
    {
        character.characterInfo.characterScripts.managementCharacterObjects.TakeObject(gameObject);
    }
    public enum TypeInteract
    {
        Item = 0,
        Object = 1
    }
    public interface IObjectInteract{
        public void Interact(Character character);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementChest : MonoBehaviour, ManagementInteract.IObjectInteract
{
    public ManagementInteract managementInteract;
    public Animator animator;
    public bool needKey = false;
    public bool isUnlock = false;
    public bool isOpen = false;
    public ManagementKey.TypeKey typeKeyChest;
    public ProbabilityItems[] probabilityItems;
    public void Interact(Character character)
    {
        if (!isOpen)
        {
            if (!needKey)
            {
                OpenChest();
            }
            else
            {
                if (ValidateUnlock(character))
                {
                    OpenChest();
                }
            }
        }
    }
    public void OpenChest()
    {
        managementInteract.canInteract = false;
        isOpen = true;
        animator.Play("ChestOpen", 0, 0f);
        List<GameObject> objectsSelected = SelectItems();
        for (int i = 0; i < objectsSelected.Count; i++)
        {
            GameObject obj = Instantiate(objectsSelected[i], transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.GetComponent<ObjectBase>().meshObj.SetActive(false);
            obj.GetComponent<ManagementInteract>().canInteract = true;
            obj.GetComponent<ObjectBase>().objectInfo.amount = 1;
        }
    }
    public int ValidateAmountItems()
    {
        int amount = 0;
        float probability = Random.Range(0, 100);
        if (probability <= 10)
        {
            amount++;
        }
        if (probability <= 20)
        {
            amount++;
        }
        if (probability <= 30)
        {
            amount++;
        }
        if (probability <= 40)
        {
            amount++;
        }
        if (probability <= 100)
        {
            amount++;
        }
        return amount;
    }
    public List<GameObject> SelectItems()
    {
        List<GameObject> objects = new List<GameObject>();
        int numberItems = Random.Range(1, 5);
        int index = 0;
        while (index < numberItems)
        {
            index++;
            Random.InitState(System.DateTime.Now.Millisecond);
            float probabilityItem = Random.Range(0, 100);
            List<ProbabilityItems> paths = new List<ProbabilityItems>();
            for (int i = 0; i < probabilityItems.Length; i++)
            {
                if (probabilityItem <= probabilityItems[i].probability)
                {
                    paths.Add(probabilityItems[i]);
                }
            }
            for (int i = 0; i < paths.Count; i++)
            {                
                GameObject[] objectsSelected = Resources.LoadAll<GameObject>($"Prefabs/Objects/{paths[i].pathObjects}");
                int indexObject = Random.Range(0, objectsSelected.Length - 1);
                if (objects.Count > 0)
                {
                    if (!objects.Contains(objectsSelected[indexObject]))
                    {
                        objects.Add(objectsSelected[indexObject]);
                    }
                }
                else
                {
                    objects.Add(objectsSelected[i]);
                }
            }
        }
        return objects;
    }
    public bool ValidateUnlock(Character character)
    {
        if (!isUnlock)
        {
            for (int i = 0; i < character.characterInfo.characterScripts.managementCharacterObjects.objects.Length; i++)
            {
                if (character.characterInfo.characterScripts.managementCharacterObjects.objects[i].objectData)
                {
                    if (character.characterInfo.characterScripts.managementCharacterObjects.objects[i].objectData.objectInstance.TryGetComponent<ManagementKey>(out ManagementKey key))
                    {
                        if (typeKeyChest == key.typeKey)
                        {
                            isUnlock = true;
                            character.characterInfo.characterScripts.managementCharacterObjects.objects[i].amount--;
                            character.characterInfo.characterScripts.managementCharacterObjects.RefreshObjects();
                            return true;
                        }
                    }
                }
            }
        }
        character.characterInfo.characterScripts.managementCharacterHud.SendInformationMessage(41, Color.red);
        return false;
    }
    [System.Serializable] public class ProbabilityItems
    {
        public string pathObjects;
        public float probability;
    }
}

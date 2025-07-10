using UnityEngine;

public class ManagementTorch : ObjectBase
{
public override void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        if (objectInfo.isUsingItem)
        {
            if (character.characterInfo.isPlayer) character.characterInfo.characterScripts.managementCharacterHud.ToggleActiveObject(objectInfo.objectPos, false);
            objectInfo.isUsingItem = false;
            Destroy(objectInfo.objectInstance);
        }

        Vector3 positionsSpawn = character.characterInfo.characterScripts.managementCharacterModelDirection.directionPlayer.transform.GetChild(0).transform.position;
        GameObject armor = Instantiate(objectInfo.objectData.objectInstance, positionsSpawn, Quaternion.identity);
        Vector3 directionForce = (character.transform.position - armor.transform.position).normalized;
        armor.GetComponent<Rigidbody>().isKinematic = false;
        armor.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        armor.GetComponent<ManagementInteract>().canInteract = true;
        this.objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterInfo.characterScripts.managementCharacterObjects.RefreshObjects();
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1, true);
    }

    public override void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        InstanceTorch(objectInfo, managementCharacterObjects);
        if (character.characterInfo.isPlayer) character.characterInfo.characterScripts.managementCharacterHud.ToggleActiveObject(objectInfo.objectPos, true);
    }

    public override void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.isUsingItem = !objectInfo.isUsingItem;
        if (objectInfo.objectInstance == null)
        {
            InstanceTorch(objectInfo, managementCharacterObjects);
            objectInfo.objectInstance.SetActive(false);
        }
        if (objectInfo.isUsingItem)
        {
            objectInfo.objectInstance.SetActive(true);
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1.1f, true);
        }
        else
        {
            objectInfo.objectInstance.SetActive(false);
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 0.9f, true);
        }
        if (character.characterInfo.isPlayer) character.characterInfo.characterScripts.managementCharacterHud.ToggleActiveObject(objectInfo.objectPos, objectInfo.isUsingItem);
        character.characterInfo.RefreshCurrentStatistics();
    }
    public void InstanceTorch(ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.objectInstance = Instantiate(objectInfo.objectData.objectInstanceForUse, managementCharacterObjects.GetObjectsPositionsInfo(ManagementCharacterObjects.TypeObjectPosition.Ligth).objectPosition.transform);
        objectInfo.objectInstance.transform.localPosition = Vector3.zero;
        objectInfo.objectInstance.transform.localRotation = Quaternion.Euler(Vector3.zero);
        objectInfo.objectInstance.transform.localScale = Vector3.one;
    }
}

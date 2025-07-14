using UnityEngine;

public class ManagementWeaponsObject : ObjectBase
{
    public bool needInstance = false;
    public string typeAnimation;
    public override void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.isUsingItem = !objectInfo.isUsingItem;
        if (objectInfo.isUsingItem)
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue += armorStats.baseValue;
            }
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1.1f, true);
            if (needInstance && character.initialData.isHuman) character.characterObjects.InstanceObjectInHand(objectInfo.objectData.objectInstance, false);
        }
        else
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue -= armorStats.baseValue;
            }
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 0.9f, true);
            character.characterObjects.DestroyObjectInHand(false);
        }
        character.RefreshCurrentStatistics();
        if (character.isPlayer)
        {
            character.characterHud.RefreshCurrentStatistics();
            character.characterHud.ToggleActiveObject(objectInfo.objectPos, objectInfo.isUsingItem);
        }
    }
    public override void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        if (objectInfo.isUsingItem)
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue -= armorStats.baseValue;
            }
            if (character.isPlayer) character.characterHud.ToggleActiveObject(objectInfo.objectPos, false);
            objectInfo.isUsingItem = false;
            character.RefreshCurrentStatistics();
            if (character.isPlayer)
            {
                character.characterHud.RefreshCurrentStatistics();
            }
            character.characterHud.RefreshCurrentStatistics();
            character.characterObjects.DestroyObjectInHand(false);
        }

        Vector3 positionsSpawn = character.characterModelDirection.directionPlayer.transform.GetChild(0).transform.position;
        GameObject armor = Instantiate(objectInfo.objectData.objectInstance, positionsSpawn, Quaternion.identity);
        Vector3 directionForce = (character.transform.position - armor.transform.position).normalized;
        armor.GetComponent<Rigidbody>().isKinematic = false;
        armor.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        armor.GetComponent<ManagementInteract>().canInteract = true;
        this.objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterObjects.RefreshObjects();
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1, true);
    }

    public override void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
        {
            Character.Statistics statistic = character.GetStatisticByType(armorStats.typeStatistics);
            statistic.objectValue += armorStats.baseValue;
        }
        character.RefreshCurrentStatistics();
        if (character.isPlayer)
        {
            character.characterHud.RefreshCurrentStatistics();
            character.characterHud.ToggleActiveObject(objectInfo.objectPos, true);
        }
        if (needInstance && character.initialData.isHuman) character.characterObjects.InstanceObjectInHand(objectInfo.objectData.objectInstance, false);
    }
    public override string GetTypeObjAnimation()
    {
        return typeAnimation;
    }
}

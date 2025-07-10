using UnityEngine;

public class ManagementArmors : ObjectBase
{
    public override void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        if (objectInfo.isUsingItem)
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue -= armorStats.baseValue;
            }
            objectInfo.isUsingItem = false;
            character.characterInfo.RefreshCurrentStatistics();
            if (character.characterInfo.isPlayer)
            {
                character.characterInfo.RefreshCurrentStatistics();
                character.characterInfo.characterScripts.managementCharacterHud.ToggleActiveObject(objectInfo.objectPos, false);
            }
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
        foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
        {
            Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
            statistic.objectValue += armorStats.baseValue;
        }
            character.characterInfo.RefreshCurrentStatistics();
            if (character.characterInfo.isPlayer)
            {
                character.characterInfo.RefreshCurrentStatistics();
                character.characterInfo.characterScripts.managementCharacterHud.ToggleActiveObject(objectInfo.objectPos, true);
            }
    }

    public override void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.isUsingItem = !objectInfo.isUsingItem;
        if (objectInfo.isUsingItem)
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue += armorStats.baseValue;
            }
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1.1f, true);
        }
        else
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue -= armorStats.baseValue;
            }
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 0.9f, true);
        }
        character.characterInfo.RefreshCurrentStatistics();
        if (character.characterInfo.isPlayer)
        {
            character.characterInfo.characterScripts.managementCharacterHud.RefreshCurrentStatistics();
            character.characterInfo.characterScripts.managementCharacterHud.ToggleActiveObject(objectInfo.objectPos, objectInfo.isUsingItem);
        }
    }
}

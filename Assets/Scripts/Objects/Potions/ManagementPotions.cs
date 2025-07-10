using UnityEngine;

public class ManagementPotions : ObjectBase
{
    public GameObject effect;
    public override void DropObject(Character character ,ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {        
        Vector3 positionsSpawn = character.characterInfo.characterScripts.managementCharacterModelDirection.directionPlayer.transform.GetChild(0).transform.position;
        GameObject potion = Instantiate(objectInfo.objectData.objectInstance, positionsSpawn, Quaternion.identity);
        Vector3 directionForce = (character.transform.position - potion.transform.position).normalized;
        potion.GetComponent<Rigidbody>().isKinematic = false;
        potion.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        potion.GetComponent<ManagementInteract>().canInteract = true;
        this.objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterInfo.characterScripts.managementCharacterObjects.RefreshObjects();
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1, true);
    }
    public override void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        foreach(Character.Statistics potion in objectInfo.objectData.statistics)
        {
            Character.Statistics statistic = character.characterInfo.GetStatisticByType(potion.typeStatistics);
            float value = objectInfo.objectData.isPorcent ? Mathf.Ceil(statistic.maxValue * potion.baseValue / 100) : potion.baseValue;
            statistic.currentValue += value;
            GameObject floatingText = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingText/FloatingText"), character.gameObject.transform.position, Quaternion.identity);
            FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
            _ = floatingTextScript.SendText(Mathf.Ceil(value).ToString(), objectInfo.objectData.colorEffect);
            if (statistic.currentValue > statistic.maxValue)
            {
                statistic.currentValue = statistic.maxValue;
            }
        }
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PotionEffect"), 1, true);
        GameObject potionEffect = Instantiate(effect, character.transform.position + new Vector3(0, 0.05f, 0), Quaternion.identity, character.transform);
        Destroy(potionEffect, 3);
        objectInfo.amount--;
        character.characterInfo.characterScripts.managementCharacterObjects.RefreshObjects();
    }
    public enum TypePotion
    {
        None = 0,
        Heal = 1,
        Mana = 2,
        Stamina = 3
    }
}

using UnityEngine;

public class ManagementKey : ObjectBase
{
    public TypeKey typeKey;
    public LayerMask layerMask;
    public AudioClip unlockClip;
    public AudioClip noUnlockClip;
    public override void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Vector3 positionsSpawn = character.characterInfo.characterScripts.managementCharacterModelDirection.directionPlayer.transform.GetChild(0).transform.position;
        GameObject objectInstance = Instantiate(objectInfo.objectData.objectInstance, positionsSpawn, Quaternion.identity, character.gameObject.transform);
        Vector3 directionForce = (character.transform.position - objectInstance.transform.position).normalized;
        objectInstance.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        objectInstance.GetComponent<ManagementInteract>().canInteract = true;
        this.objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterInfo.characterScripts.managementCharacterObjects.RefreshObjects();
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("PickUp"), 1, true);
    }
    public enum TypeKey
    {
        None = 0,
        General = 1,
        Special = 2
    }
}

using UnityEngine;

public class KeyListener : MonoBehaviour, KeyListener.IKey
{
    public ManagementKey.TypeKey typeKey;
    public GameObject bannerUseKey;
    public GameObject bannerInteract;
    public bool needKey = false;
    public bool isUnLock = false;
    public bool ValidateUnlock(ManagementKey.TypeKey currentKey)
    {
        if (!isUnLock)
        {
            if (typeKey == currentKey)
            {
                isUnLock = true;
                return true;
            }
        }
        return false;
    }
    public bool ValidateIsUnlock()
    {
        return isUnLock;
    }

    public void UseKey()
    {
        GetComponent<IUseKey>().UseKey();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isUnLock)
            {
                if (needKey)
                {
                    bannerInteract.SetActive(true);
                    bannerUseKey.SetActive(false);
                }
                else
                {
                    bannerInteract.SetActive(false);
                    bannerUseKey.SetActive(true);
                }
            }
            else
            {
                bannerInteract.SetActive(false);
                bannerUseKey.SetActive(false);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bannerInteract.SetActive(false);
            bannerUseKey.SetActive(false);
        }
    }
    public interface IKey
    {
        public bool ValidateIsUnlock();
        public bool ValidateUnlock(ManagementKey.TypeKey typeKey);
        public void UseKey();
    }
    public interface IUseKey
    {
        public void UseKey();
    }
}

using System;
using System.Linq;
using System.Security.Cryptography;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ManagementStatusEffect : MonoBehaviour
{
    public Character character;
    public SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData> statusEffects = new SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData>();
    public TestEffects testEffects;

    [NaughtyAttributes.Button]
    public void Test()
    {
        testEffects.TestEffect();
    }
    void Update()
    {
        if (character.isInitialize && GameManager.Instance.startGame)
        {
            if (statusEffects.Count > 0)
            {
                for (int i = 0; i < statusEffects.Count; i++)
                {
                    StatusEffectsData status = statusEffects.ElementAt(i).Value;
                    status.currentTime -= Time.deltaTime;
                    if (status.currentTime <= 0)
                    {
                        if (!status.statusEffectSO.increaseTime)
                        {
                            status.currentAccumulations--;
                            status.currentTime = status.statusEffectSO.timePerAccumulation;
                            if (status.currentAccumulations > 0)
                            {
                                status.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().IncreaseAccumulation(status, status.objectMakeEffect, status.objectTakeEffect);
                                character.characterHud.UpdateStatusEffect(statusEffects[status.statusEffectSO.typeStatusEffect]);
                            }
                            else
                            {
                                character.characterHud.DestroyStatusEffect(status.statusEffectSO.typeStatusEffect);
                                statusEffects.Remove(status.statusEffectSO.typeStatusEffect);

                                status.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().Finish(status, status.objectMakeEffect, status.objectTakeEffect);
                            }
                        }
                        else
                        {
                            status.currentAccumulations++;
                            status.currentTime = status.statusEffectSO.timePerAccumulation;
                            if (status.currentAccumulations <= status.statusEffectSO.maxAccumulations)
                            {
                                status.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().IncreaseAccumulation(status, status.objectMakeEffect, status.objectTakeEffect);
                                character.characterHud.UpdateStatusEffect(statusEffects[status.statusEffectSO.typeStatusEffect]);
                            }
                            else
                            {
                                character.characterHud.DestroyStatusEffect(status.statusEffectSO.typeStatusEffect);
                                statusEffects.Remove(status.statusEffectSO.typeStatusEffect);
                                status.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().Finish(status, status.objectMakeEffect, status.objectTakeEffect);
                            }
                        }
                    }
                }
            }
        }
    }
    public void AddStatus(StatusEffectSO statusEffectSO, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (statusEffects.TryGetValue(statusEffectSO.typeStatusEffect, out StatusEffectsData statusEffectsData))
        {
            if (!statusEffectsData.statusEffectSO.increaseTime)
            {
                statusEffectsData.currentAccumulations += statusEffectSO.amountPerAccumulation;
            }
            else
            {
                statusEffectsData.currentAccumulations = 1;
            }

            statusEffectsData.currentTime = statusEffectSO.timePerAccumulation;

            if (statusEffectsData.currentAccumulations > statusEffectSO.maxAccumulations)
            {
                statusEffectsData.currentAccumulations = statusEffectSO.maxAccumulations;
                statusEffectsData.currentTime = statusEffectSO.timePerAccumulation;
                statusEffectsData.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().AllAccumulationsReached(statusEffectsData, objectMakeEffect, objectTakeEffect);
            }
            else
            {
                statusEffectsData.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().ReApply(statusEffectsData, objectMakeEffect, objectTakeEffect);
            }
        }
        else
        {
            statusEffects.Add(statusEffectSO.typeStatusEffect, new StatusEffectsData(statusEffectSO, statusEffectSO.timePerAccumulation, statusEffectSO.amountPerAccumulation, objectMakeEffect, objectTakeEffect));
            statusEffects.TryGetValue(statusEffectSO.typeStatusEffect, out StatusEffectsData newEffectData);
            statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().Apply(newEffectData, objectMakeEffect, objectTakeEffect);
        }
        character.characterHud.UpdateStatusEffect(statusEffects[statusEffectSO.typeStatusEffect]);
    }
    public bool StatusEffectExist(StatusEffectSO.TypeStatusEffect typeStatusEffect)
    {
        return statusEffects.ContainsKey(typeStatusEffect);
    }
    public bool GetStatusEffect(StatusEffectSO.TypeStatusEffect typeStatusEffect, out StatusEffectsData statusEffectsData)
    {
        if (statusEffects.TryGetValue(typeStatusEffect, out StatusEffectsData statusEffect))
        {
            statusEffectsData = statusEffect;
            return true;
        }
        statusEffectsData = null;
        return false;
    }
    [NaughtyAttributes.Button]
    public void CleanAllDebuffs()
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            StatusEffectsData status = statusEffects.ElementAt(i).Value;
            if (status.statusEffectSO.GetTypeEffect(status.currentAccumulations) == StatusEffectSO.TypeEffect.Debuff)
            {
                character.characterHud.DestroyStatusEffect(status.statusEffectSO.typeStatusEffect);
                statusEffects.Remove(status.statusEffectSO.typeStatusEffect);
                status.statusEffectSO.statusEffectInstance.GetComponent<StatusEffectBase>().Clean(status, status.objectMakeEffect, status.objectTakeEffect);
            }
        }
    }
    [Serializable]
    public class StatusEffectsData
    {
        public StatusEffectSO statusEffectSO;
        public float currentTime = 0;
        public int currentAccumulations = 0;
        public bool statisticsApply;
        public GameObject objectMakeEffect;
        public GameObject objectTakeEffect;
        public StatusEffectsData(StatusEffectSO statusEffectSO, float currentTime, int currentTimeAccumulations, GameObject objectMakeEffect, GameObject objectTakeEffect)
        {
            this.statusEffectSO = statusEffectSO;
            this.currentTime = currentTime;
            this.currentAccumulations = currentTimeAccumulations;
            this.objectMakeEffect = objectMakeEffect;
            this.objectTakeEffect = objectTakeEffect;
        }
    }
    [Serializable]
    public class TestEffects
    {
        public ManagementStatusEffect managementStatusEffect;
        public StatusEffectSO statusEffectSO;
        public GameObject objectMakeEffect;
        public GameObject objectTakeEffect;
        public void TestEffect()
        {

            if (!statusEffectSO)
            {
                Debug.LogError("Es necesario un efecto de estado");
                return;
            }

            if (objectMakeEffect && objectTakeEffect)
            {
                managementStatusEffect.AddStatus(statusEffectSO, objectMakeEffect, objectTakeEffect);
            }
            else
            {
                Debug.LogError("No se tienen los campos necesarios para testear el efecto");
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ManagementStatusEffect : MonoBehaviour
{
    public Character character;
public SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData> statusEffects = new SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData>();
    void Update()
    {
        if (character.characterInfo.isActive)
        {
            if (statusEffects.Count > 0)
            {
                for (int i = 0; i < statusEffects.Count; i++)
                {
                    StatusEffectsData status = statusEffects.ElementAt(i).Value;
                    status.currentTime -= Time.deltaTime;
                    if (status.currentTime <= 0)
                    {
                        character.characterInfo.characterScripts.managementCharacterHud.DestroyStatusEffect(status.statusEffectSO.typeStatusEffect);
                        statusEffects.Remove(status.statusEffectSO.typeStatusEffect);
                    }
                    else
                    {
                        status.currentAccumulations = (int)Math.Ceiling(status.currentTime / status.statusEffectSO.timePerAcumulation);
                    }
                }
            }
        }
    }
    public void AddStatus(StatusEffectSO statusEffectSO, GameObject objectMakeEffect = null, GameObject objectToMakeEffect = null){
        if (statusEffects.TryGetValue(statusEffectSO.typeStatusEffect, out StatusEffectsData statusEffectsData)){
            if (statusEffectsData.currentTime + statusEffectSO.timePerAcumulation > statusEffectSO.timePerAcumulation * statusEffectSO.maxAccumulations)
            {
                statusEffectsData.currentAccumulations = statusEffectSO.maxAccumulations;
                statusEffectsData.currentTime = statusEffectSO.timePerAcumulation * statusEffectSO.maxAccumulations;                
            }
            else
            {
                statusEffectsData.currentAccumulations++;
                statusEffectsData.currentTime += statusEffectSO.timePerAcumulation;
            }
        }
        else
        {
            Coroutine effectCoroutine = StartCoroutine(statusEffectSO.statusEffectInstance.GetComponent<IStatusEffect>().ApplyStatusEffect(objectMakeEffect, objectToMakeEffect));
            statusEffects.Add(statusEffectSO.typeStatusEffect, new StatusEffectsData (statusEffectSO, statusEffectSO.timePerAcumulation, 1, effectCoroutine));
            character.characterInfo.characterScripts.managementCharacterHud.UpdateStatusEffect(statusEffects[statusEffectSO.typeStatusEffect]);
        }
    }
    [Serializable] public class StatusEffectsData
    {
        public StatusEffectSO statusEffectSO;
        public float currentTime = 0;
        public int currentAccumulations = 0;
        public Coroutine effectCoroutine = null;
        public StatusEffectsData( StatusEffectSO statusEffectSO, float currentTime, int currentAccumulations, Coroutine effectCoroutine)
        {
            this.statusEffectSO = statusEffectSO;
            this.currentTime = currentTime;
            this.currentAccumulations = currentAccumulations;
            this.effectCoroutine = effectCoroutine;
        }
    }
    public interface IStatusEffect
    {
        public IEnumerator ApplyStatusEffect(GameObject objectMakeEffect, GameObject objectToMakeEffect);
    }
}

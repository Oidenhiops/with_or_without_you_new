using UnityEngine;

public class DecreaseAtkSpd : StatusEffectBase
{
    public override void Apply(GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            if (characterTakeEffect.statusEffect.GetStatusEffect(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed, out ManagementStatusEffect.StatusEffectsData statusEffectsData))
            {
                if (statusEffectsData.currentAccumulations == statusEffectsData.statusEffectSO.maxAccumulations)
                {
                    characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffValue -= 40f;
                }
                else
                {
                    characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffValue -= 10f;
                }
            }
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
    public override void ReApply(GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Apply(objectMakeEffect, objectTakeEffect);
    }
    public override void DecreaseAccumulation(GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Finish(objectMakeEffect, objectTakeEffect);
    }
    public override void Finish(GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffValue += 10f;
            if (characterTakeEffect.statusEffect.GetStatusEffect(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed, out ManagementStatusEffect.StatusEffectsData statusEffectsData))
            {
                if (statusEffectsData.currentAccumulations == statusEffectsData.statusEffectSO.maxAccumulations - 1)
                {
                    characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffValue += 30f;
                }
            }
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
}
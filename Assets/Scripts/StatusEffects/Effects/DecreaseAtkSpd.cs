using UnityEngine;

public class DecreaseAtkSpd : StatusEffectBase
{
    public override void Apply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed, out Character.BuffStatistic buffStatistic))
            {
                buffStatistic.valuePorcent = -10 * GetCharges(statusEffectsData);
            }
            else
            {
                characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.Add(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed, new Character.BuffStatistic(StatusEffectSO.TypeEffect.Debuff, 0, -10 * GetCharges(statusEffectsData)));
            }
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
    int GetCharges(ManagementStatusEffect.StatusEffectsData statusEffectsData)
    {
        return statusEffectsData.currentAccumulations == statusEffectsData.statusEffectSO.maxAccumulations ? 8 : statusEffectsData.currentAccumulations;
    }
    public override void ReApply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed, out Character.BuffStatistic buffStatistic))
            {
                buffStatistic.valuePorcent = -10 * GetCharges(statusEffectsData);
            }
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
    public override void DecreaseAccumulation(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed, out Character.BuffStatistic buffStatistic))
            {
                buffStatistic.valuePorcent = -10 * GetCharges(statusEffectsData);
            }
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
    public override void Finish(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.Remove(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed);
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
    public override void Clean(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.Remove(StatusEffectSO.TypeStatusEffect.DecreaseAtkSpeed);
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
}
using UnityEngine;

public class BerserkEffect : StatusEffectBase
{
    public override void Apply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.Atk).buffStatistic.ContainsKey(StatusEffectSO.TypeStatusEffect.Berserk) &&
                characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.ContainsKey(StatusEffectSO.TypeStatusEffect.Berserk))
            {
                if (statusEffectsData.currentAccumulations == 1)
                {
                    if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.Atk).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.Berserk, out Character.BuffStatistic berserkAtkBuff))
                    {
                        berserkAtkBuff.typeEffect = StatusEffectSO.TypeEffect.Buff;
                        berserkAtkBuff.valuePorcent = 50f;
                    }
                    if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.Berserk, out Character.BuffStatistic berserkAtkSpdBuff))
                    {
                        berserkAtkSpdBuff.typeEffect = StatusEffectSO.TypeEffect.Buff;
                        berserkAtkSpdBuff.valuePorcent = 50f;
                    }
                }
                else
                {
                    if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.Atk).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.Berserk, out Character.BuffStatistic berserkAtkBuff))
                    {
                        berserkAtkBuff.typeEffect = StatusEffectSO.TypeEffect.Debuff;
                        berserkAtkBuff.valuePorcent = -50f;
                    }
                    if (characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.TryGetValue(StatusEffectSO.TypeStatusEffect.Berserk, out Character.BuffStatistic berserkAtkSpdBuff))
                    {
                        berserkAtkSpdBuff.typeEffect = StatusEffectSO.TypeEffect.Debuff;
                        berserkAtkSpdBuff.valuePorcent = -50f;
                    }
                }
            }
            else
            {
                characterTakeEffect.GetStatisticByType(Character.TypeStatistics.Atk).buffStatistic.Add(StatusEffectSO.TypeStatusEffect.Berserk, new Character.BuffStatistic(StatusEffectSO.TypeEffect.Buff, 0, 50));
                characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.Add(StatusEffectSO.TypeStatusEffect.Berserk, new Character.BuffStatistic(StatusEffectSO.TypeEffect.Buff, 0, 50));
            }
            characterTakeEffect.RefreshCurrentStatistics();
            if (characterTakeEffect.isPlayer)
            {
                characterTakeEffect.characterHud.RefreshCurrentStatistics();
            }
        }
    }
    public override void ReApply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Apply(statusEffectsData, objectMakeEffect, objectTakeEffect);
    }
    public override void IncreaseAccumulation(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Apply(statusEffectsData, objectMakeEffect, objectTakeEffect);
    }
    public override void Finish(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            characterTakeEffect.GetStatisticByType(Character.TypeStatistics.Atk).buffStatistic.Remove(StatusEffectSO.TypeStatusEffect.Berserk);
            characterTakeEffect.GetStatisticByType(Character.TypeStatistics.AtkSpd).buffStatistic.Remove(StatusEffectSO.TypeStatusEffect.Berserk);
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
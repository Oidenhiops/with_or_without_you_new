using System.Collections;
using UnityEngine;

public class BleedingEffect : StatusEffectBase
{
    public StatusEffectSO statusEffectSO;
    [Range(0f, 1f)]
    public float porcentDamage;
    public override void Apply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            objectMakeEffect.TryGetComponent<Character>(out Character characterMakeEffect);
            Bleeding(characterMakeEffect, characterTakeEffect);
        }
    }
    public override void AllAccumulationsReached(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Bleeding(objectMakeEffect.GetComponent<Character>(), objectTakeEffect.GetComponent<Character>());
    }
    public override void DecreaseAccumulation(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Bleeding(objectMakeEffect.GetComponent<Character>(), objectTakeEffect.GetComponent<Character>());
    }
    public override void Finish(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Bleeding(objectMakeEffect.GetComponent<Character>(), objectTakeEffect.GetComponent<Character>());
    }
    public void Bleeding(Character characterMakeEffect, Character characterTakeEffect)
    {
        characterTakeEffect.TakeDamage(characterTakeEffect.GetStatisticByType(Character.TypeStatistics.Hp).maxValue * porcentDamage, characterTakeEffect.colorBlood, 0.1f, Character.TypeDamage.TrueDamage, characterMakeEffect);
    }
}

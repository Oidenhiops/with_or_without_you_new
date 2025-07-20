using UnityEngine;

public class DisarmEffect : StatusEffectBase
{
    public override void Apply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            foreach (ManagementCharacterObjects.ObjectsInfo weapon in characterTakeEffect.characterObjects.FindObjectsByType(ItemsDataSO.TypeObject.Weapon))
            {
                if (weapon.isUsingItem)
                {
                    characterTakeEffect.characterObjects.DropObjectByPos(weapon.objectPos);
                    if (characterTakeEffect.characterAnimations.currentAnimation.typeAnimation == CharacterAnimationsSO.TypeAnimation.Attack)
                    {
                        characterTakeEffect.characterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "Idle");
                    }
                }
            }
        }
    }
    public override void ReApply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Apply(statusEffectsData, objectMakeEffect, objectTakeEffect);
    }
}

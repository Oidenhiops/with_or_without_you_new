using UnityEngine;

public class PushEffect : StatusEffectBase
{
    public StatusEffectSO statusEffectSO;
    public string idForce;
    public float pushForce = 5;
    public override void Apply(GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        if (objectTakeEffect.TryGetComponent<Character>(out Character characterTakeEffect))
        {
            Vector3 direction = (objectTakeEffect.transform.position - objectMakeEffect.transform.position).normalized;
            characterTakeEffect.characterMove.AddOtherForce(idForce, direction * pushForce, true, statusEffectSO.timePerAccumulation);
        }
    }
    public override void ReApply(GameObject objectMakeEffect, GameObject objectTakeEffect)
    {
        Apply(objectMakeEffect, objectTakeEffect);
    }
}
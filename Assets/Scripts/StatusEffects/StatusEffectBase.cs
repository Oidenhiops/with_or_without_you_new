using UnityEngine;

public class StatusEffectBase : MonoBehaviour
{
    public virtual void CancelStatusEffect(ManagementStatusEffect.StatusEffectsData statusEffectsData) { Debug.Log("Cancel Not Implemented"); }
    public virtual void Apply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Apply Not Implemented"); }
    public virtual void ReApply(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("ReApply Coroutine Not Implemented"); }
    public virtual void AllAccumulationsReached(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("All Accumulations Reached Not Implemented"); }
    public virtual void DecreaseAccumulation(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Decrease Accumulation Not Implemented"); }
    public virtual void IncreaseAccumulation(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Decrease Accumulation Not Implemented"); }
    public virtual void Finish(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Finish Not Implemented"); }
    public virtual void Clean(ManagementStatusEffect.StatusEffectsData statusEffectsData, GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Clean Not Implemented"); }
}
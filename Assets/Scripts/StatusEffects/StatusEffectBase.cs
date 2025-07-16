using UnityEngine;

public class StatusEffectBase : MonoBehaviour
{
    public virtual void CancelStatusEffect() { Debug.Log("Cancel Not Implemented"); }
    public virtual void Apply(GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Apply Not Implemented"); }
    public virtual void ReApply(GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("ReApply Coroutine Not Implemented"); }
    public virtual void AllAccumulationsReached(GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("All Accumulations Reached Not Implemented"); }
    public virtual void DecreaseAccumulation(GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Decrease Accumulation Not Implemented"); }
    public virtual void Finish(GameObject objectMakeEffect, GameObject objectTakeEffect) { Debug.Log("Finish Not Implemented"); }
}
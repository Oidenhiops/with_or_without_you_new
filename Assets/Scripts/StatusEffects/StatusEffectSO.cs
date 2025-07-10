using UnityEngine;
[CreateAssetMenu(fileName = "StatusEffectData", menuName = "ScriptableObjects/StatusEffects/StatusEffectDataSO", order = 1)]
public class StatusEffectSO : ScriptableObject
{
    public TypeStatusEffect typeStatusEffect;
    public GameObject statusEffectInstance;
    public int maxAccumulations;
    public float timePerAcumulation;
    public enum TypeStatusEffect
    {
        None = 0,
        Push = 1,
    }
    public enum TypeEffect
    {
        None = 0,
        Buff = 1,
        Debuff = 2
    }
}

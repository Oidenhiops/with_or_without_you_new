using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "StatusEffectData", menuName = "ScriptableObjects/StatusEffects/StatusEffectDataSO", order = 1)]
public class StatusEffectSO : ScriptableObject
{
    public Sprite spriteStatusEffect;
    public TypeStatusEffect typeStatusEffect;
    public TypeEffect typeEffect;
    public GameObject statusEffectInstance;
    public int maxAccumulations;
    public float timePerAccumulation;
    public enum TypeStatusEffect
    {
        None = 0,
        Push = 1,
        Disarm = 2,
        Bleeding = 3,
        DecreaseAtkSpeed = 4
    }
    public enum TypeEffect
    {
        None = 0,
        Buff = 1,
        Debuff = 2
    }
}

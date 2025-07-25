using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "StatusEffectData", menuName = "ScriptableObjects/StatusEffects/StatusEffectDataSO", order = 1)]
public class StatusEffectSO : ScriptableObject
{
    public Sprite[] spriteStatusEffect;
    public TypeStatusEffect typeStatusEffect;
    public TypeEffect[] typeEffect;
    public GameObject statusEffectInstance;
    public bool increaseTime;
    public int maxAccumulations = 0;
    public float timePerAccumulation = 0;
    public int amountPerAccumulation = 1;
    public enum TypeStatusEffect
    {
        None = 0,
        Push = 1,
        Disarm = 2,
        Bleeding = 3,
        DecreaseAtkSpeed = 4,
        Berserk = 5
    }
    public enum TypeEffect
    {
        None = 0,
        Buff = 1,
        Debuff = 2
    }
    public TypeEffect GetTypeEffect(int index)
    {
        return typeEffect.Length > 1 ? typeEffect[index] : typeEffect[0];
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUiHelper : MonoBehaviour
{
    public Image statusEffectFill;
    public Image statusEffectImage;
    public TMP_Text statusEffectAccumulations;

    public void SetInfo(ManagementStatusEffect.StatusEffectsData statusEffectsData)
    {
        statusEffectImage.sprite = statusEffectsData.statusEffectSO.spriteStatusEffect;
        statusEffectAccumulations.text = statusEffectsData.currentAccumulations.ToString();
    }
    public void UpdateInfo(ManagementStatusEffect.StatusEffectsData statusEffectsData)
    {
        statusEffectAccumulations.text = statusEffectsData.currentAccumulations > 1 ? statusEffectsData.currentAccumulations.ToString() : "";
    }
}

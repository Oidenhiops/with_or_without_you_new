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
        statusEffectImage.sprite = statusEffectsData.statusEffectSO.spriteStatusEffect.Length > 1 ? statusEffectsData.statusEffectSO.spriteStatusEffect[statusEffectsData.currentAccumulations - 1] : statusEffectsData.statusEffectSO.spriteStatusEffect[0];
        statusEffectAccumulations.text = statusEffectsData.currentAccumulations.ToString();
    }
    public void UpdateInfo(ManagementStatusEffect.StatusEffectsData statusEffectsData)
    {
        statusEffectImage.sprite = statusEffectsData.statusEffectSO.spriteStatusEffect.Length > 1 ? statusEffectsData.statusEffectSO.spriteStatusEffect[statusEffectsData.currentAccumulations - 1] : statusEffectsData.statusEffectSO.spriteStatusEffect[0];
        statusEffectAccumulations.text = statusEffectsData.currentAccumulations > 1 ? statusEffectsData.currentAccumulations.ToString() : "";
    }
}

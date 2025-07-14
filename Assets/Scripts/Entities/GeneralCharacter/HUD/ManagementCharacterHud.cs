using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine.EventSystems;

public class ManagementCharacterHud : MonoBehaviour
{
    public PlayerInputs playerInputs;
    public Character character;
    public CharacterUi characterUi;
    public void InitializeHud()
    {
        if (character.isPlayer)
        {
            GameManager.Instance.OnDeviceChanged += EnabledMobileHUD;
            EnabledMobileHUD(GameManager.Instance.currentDevice);
        }
    }
    void OnDestroy()
    {
        if (character.isPlayer)
        {
            GameManager.Instance.OnDeviceChanged -= EnabledMobileHUD;
        }
    }
    void EnabledMobileHUD(GameManager.TypeDevice typeDevice)
    {
        bool isActive = typeDevice == GameManager.TypeDevice.MOBILE;
        foreach (GameObject hud in characterUi.mobileHud)
        {
            hud.SetActive(isActive);
        }
    }
    public void ToggleSecondaryAction(bool value)
    {
        characterUi.objectsUi.containerObjects.color = !value ? Color.white : characterUi.objectsUi.containerObjectsSecondaryActionColor;
    }
    public void Update()
    {
        if (character.isActive)
        {
            if (characterUi.hudUi.healthBarFront != null)
            {
                characterUi.hudUi.healthBarFront.fillAmount = Mathf.Lerp(characterUi.hudUi.healthBarFront.fillAmount, character.GetStatisticByType(Character.TypeStatistics.Hp).currentValue / character.GetStatisticByType(Character.TypeStatistics.Hp).maxValue, 1.5f * Time.deltaTime);
                if (characterUi.hudUi.healthBarBack.fillAmount >= characterUi.hudUi.healthBarFront.fillAmount)
                {
                    characterUi.hudUi.healthBarBack.fillAmount = Mathf.Lerp(characterUi.hudUi.healthBarBack.fillAmount, character.GetStatisticByType(Character.TypeStatistics.Hp).currentValue / character.GetStatisticByType(Character.TypeStatistics.Hp).maxValue, 1 * Time.deltaTime);
                }
                else
                {
                    characterUi.hudUi.healthBarBack.fillAmount = characterUi.hudUi.healthBarFront.fillAmount;
                }
                if (characterUi.hudUi.healthText != null)
                {
                    characterUi.hudUi.healthText.text = $"{character.GetStatisticByType(Character.TypeStatistics.Hp).currentValue}/{character.GetStatisticByType(Character.TypeStatistics.Hp).maxValue}";
                }
            }
            if (characterUi.hudUi.manaBarFront != null)
            {
                characterUi.hudUi.manaBarFront.fillAmount = Mathf.Lerp(characterUi.hudUi.manaBarFront.fillAmount, character.GetStatisticByType(Character.TypeStatistics.Mp).currentValue / character.GetStatisticByType(Character.TypeStatistics.Mp).maxValue, 1.5f * Time.deltaTime);
                if (characterUi.hudUi.manaBarBack.fillAmount >= characterUi.hudUi.manaBarFront.fillAmount)
                {
                    characterUi.hudUi.manaBarBack.fillAmount = Mathf.Lerp(characterUi.hudUi.manaBarBack.fillAmount, character.GetStatisticByType(Character.TypeStatistics.Mp).currentValue / character.GetStatisticByType(Character.TypeStatistics.Mp).maxValue, 1 * Time.deltaTime);
                }
                else
                {
                    characterUi.hudUi.manaBarBack.fillAmount = characterUi.hudUi.manaBarFront.fillAmount;
                }
                if (characterUi.hudUi.manaText != null)
                {
                    characterUi.hudUi.manaText.text = $"{character.GetStatisticByType(Character.TypeStatistics.Mp).currentValue}/{character.GetStatisticByType(Character.TypeStatistics.Mp).maxValue}";
                }
            }
            if (characterUi.hudUi.staminaBarFront != null)
            {
                characterUi.hudUi.staminaBarFront.fillAmount = Mathf.Lerp(characterUi.hudUi.staminaBarFront.fillAmount, character.GetStatisticByType(Character.TypeStatistics.Sp).currentValue / character.GetStatisticByType(Character.TypeStatistics.Sp).maxValue, 1.5f * Time.deltaTime);
                if (characterUi.hudUi.staminaBarBack.fillAmount >= characterUi.hudUi.staminaBarFront.fillAmount)
                {
                    characterUi.hudUi.staminaBarBack.fillAmount = Mathf.Lerp(characterUi.hudUi.staminaBarBack.fillAmount, character.GetStatisticByType(Character.TypeStatistics.Sp).currentValue / character.GetStatisticByType(Character.TypeStatistics.Sp).maxValue, 1 * Time.deltaTime);
                }
                else
                {
                    characterUi.hudUi.staminaBarBack.fillAmount = characterUi.hudUi.staminaBarFront.fillAmount;
                }
                if (characterUi.hudUi.staminaText != null)
                {
                    characterUi.hudUi.staminaText.text = $"{character.GetStatisticByType(Character.TypeStatistics.Sp).currentValue}/{character.GetStatisticByType(Character.TypeStatistics.Sp).maxValue}";
                }
            }
            if (characterUi.statusEffectsUi.statusEffectsData.Count > 0)
            {
                foreach (var status in characterUi.statusEffectsUi.statusEffectsData)
                {
                    status.Value.statusEffectUi.statusEffectAccumulations.text = status.Value.statusEffectsData.currentAccumulations > 1 ? status.Value.statusEffectsData.currentAccumulations.ToString() : "";
                    float realValueValue = status.Value.statusEffectsData.currentTime - status.Value.statusEffectsData.statusEffectSO.timePerAcumulation * (status.Value.statusEffectsData.currentAccumulations - 1);
                    status.Value.statusEffectUi.statusEffectFill.fillAmount = realValueValue / status.Value.statusEffectsData.statusEffectSO.timePerAcumulation;
                }
            }
            if (characterUi.windUpFillAmount != null)
            {
                characterUi.windUpFillAmount.fillAmount = 1 - character.characterAttack.cooldownAttack;
            }
            if (characterUi.inventoryUi.inventoryAnimator != null && characterUi.inventoryUi.elapsedTime > 0)
            {
                characterUi.inventoryUi.elapsedTime -= Time.deltaTime;
                if (characterUi.inventoryUi.elapsedTime <= 0)
                {
                    ToggleShowInventory(false);
                    playerInputs.characterActionsInfo.isShowInventory = false;
                }
            }
        }
    }
    public void SendInformationMessage(int id, Color color, GameData.TypeLOCS typeLOCS)
    {
        InformationMessages message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InformationMessage/InformationMessage"), characterUi.informationMessageUi.containerInformationMessage.transform).GetComponent<InformationMessages>();
        message.textMessage.color = color;
        message.managementLanguage.id = id;
        message.managementLanguage.typeLOCS = typeLOCS;
        message.managementLanguage.RefreshText(GameData.Instance.saveData.configurationsInfo.currentLanguage);
        Destroy(message, 3);
    }
    public void SendInformationMessage(string[] ids, Color color)
    {
        InformationMessages message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InformationMessage/InformationMessage"), characterUi.informationMessageUi.containerInformationMessage.transform).GetComponent<InformationMessages>();
        message.textMessage.color = color;
        message.managementLanguage.dialogIds = ids;
        message.managementLanguage.RefreshText(GameData.Instance.saveData.configurationsInfo.currentLanguage);
        Destroy(message, 3);
    }
    public void ActiveItemsPos(int objectIndex)
    {
        for (int i = 0; i < characterUi.objectsUi.objects.Length; i++)
        {
            if (i == objectIndex)
            {
                characterUi.objectsUi.objects[i].objectBackground.color = Color.yellow;
            }
            else
            {
                characterUi.objectsUi.objects[i].objectBackground.color = Color.white;
            }
        }
    }
    public void DisableItemPos()
    {
        for (int i = 0; i < characterUi.objectsUi.objects.Length; i++)
        {
            characterUi.objectsUi.objects[i].objectBackground.color = Color.white;
        }
    }
    public void RefreshObjects(ManagementCharacterObjects.ObjectsInfo[] objects)
    {
        for (int i = 0; i < characterUi.objectsUi.objects.Length; i++)
        {
            if (objects[i].objectData != null)
            {
                characterUi.objectsUi.objects[i].spriteObject.sprite = objects[i].objectData.objectSprite;
                characterUi.objectsUi.objects[i].spriteObject.gameObject.SetActive(true);
                if (objects[i].amount > 1)
                {
                    characterUi.objectsUi.objects[i].amount.gameObject.SetActive(true);
                }
                else
                {
                    characterUi.objectsUi.objects[i].amount.gameObject.SetActive(false);
                }
                characterUi.objectsUi.objects[i].amount.text = objects[i].amount.ToString();
            }
            else
            {
                characterUi.objectsUi.objects[i].spriteObject.gameObject.SetActive(false);
                characterUi.objectsUi.objects[i].amount.gameObject.SetActive(false);
            }
        }
    }
    public void HighlightSkills(bool state)
    {
        foreach (SkillsData skill in characterUi.skillsUi.skills)
        {
            skill.skillBackground.color = state ? characterUi.skillsUi.highlightColor : Color.white;
        }
    }
    public void ToggleActiveObject(int pos, bool state)
    {
        characterUi.objectsUi.objects[pos].usingObjectSprite.gameObject.SetActive(state);
    }
    public async Awaitable RefreshInteracts(GameObject[] objectsForTake)
    {
        try
        {
            foreach (Transform child in characterUi.interactUi.containerInteract)
            {
                Destroy(child.gameObject);
            }
            characterUi.interactUi.interacts.Clear();
            for (int i = 0; i < objectsForTake.Length; i++)
            {
                BannerInteract bannerInteract = Instantiate(Resources.Load<GameObject>("Prefabs/UI/BannerInteract/BannerInteract"), characterUi.interactUi.containerInteract).GetComponent<BannerInteract>();
                bannerInteract.name = $"BannerInteract {i}";
                ObjectsForTake objectForTake = new ObjectsForTake
                {
                    interact = objectsForTake[i],
                    bannerInteract = bannerInteract
                };
                bannerInteract.managementCharacterObjects = character.characterObjects;
                bannerInteract.managementCharacterInteract = character.characterInteract;
                bannerInteract.onObjectSelect.container = characterUi.interactUi.containerInteract;
                bannerInteract.onObjectSelect.viewport = characterUi.interactUi.viewportInteract;
                bannerInteract.onObjectSelect.scrollRect = characterUi.interactUi.interactScrollRect;
                bannerInteract.objectForTake = objectsForTake[i];
                bannerInteract.managementLanguage.typeLOCS = objectForTake.interact.GetComponent<ManagementInteract>().typeInteract == ManagementInteract.TypeInteract.Item ? GameData.TypeLOCS.Items : GameData.TypeLOCS.Dialogs;
                bannerInteract.textObject.gameObject.SetActive(true);
                if (objectsForTake[i].TryGetComponent<ObjectBase>(out ObjectBase managementObject))
                {
                    bannerInteract.spriteObject.sprite = managementObject.objectInfo.objectData.objectSprite;
                    bannerInteract.managementLanguage.id = managementObject.managementInteract.IDText;
                    bannerInteract.managementLanguage.RefreshText();
                }
                else if (objectsForTake[i].TryGetComponent<ManagementInteract>(out ManagementInteract managementInteract))
                {
                    bannerInteract.managementLanguage.id = managementInteract.IDText;
                    bannerInteract.managementLanguage.RefreshText();
                }
                characterUi.interactUi.interacts.Add(objectForTake);
            }
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    public void UpdateScrollInteract()
    {
        if (character.characterInteract.currentInteracts.Length > 0)
        {
            if (character.characterInteract.currentInteractIndex > character.characterInteract.currentInteracts.Length - 1)
            {
                character.characterInteract.currentInteractIndex = character.characterInteract.currentInteracts.Length - 1;
            }
            characterUi.interactUi.interacts[character.characterInteract.currentInteractIndex].bannerInteract.GetComponent<OnObjectSelect>().ScrollTo(character.characterInteract.currentInteractIndex);
            for (int i = 0; i < characterUi.interactUi.interacts.Count; i++)
            {
                if (i == character.characterInteract.currentInteractIndex)
                {
                    characterUi.interactUi.interacts[i].bannerInteract.EnableButton();
                }
                else
                {
                    characterUi.interactUi.interacts[i].bannerInteract.DisableButton();
                }
            }
        }
        else
        {
            character.characterInteract.currentInteractIndex = 0;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    public void RefreshCurrentStatistics()
    {
        foreach (StatisticsData statisticsData in characterUi.statisticsUi.statistics)
        {
            statisticsData.amount.text = character.GetStatisticByType(statisticsData.typeStatistic).currentValue.ToString();
        }
    }
    public void RefreshSkillsSprites(ManagementCharacterSkills.SkillInfo[] skills)
    {
        for (int i = 0; i < characterUi.skillsUi.skills.Length; i++)
        {
            if (skills[i].skillData != null)
            {
                characterUi.skillsUi.skills[i].skillSprite.sprite = skills[i].skillData.skillSprite;
                characterUi.skillsUi.skills[i].skillSprite.gameObject.SetActive(true);
            }
            else
            {
                characterUi.skillsUi.skills[i].skillSprite.gameObject.SetActive(false);
            }
        }
    }
    public void RefreshSkillsTimer(ManagementCharacterSkills.SkillInfo[] skills)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].skillData != null)
            {
                characterUi.skillsUi.skills[i].skillTimer.text = skills[i].cdInfo.currentCD > 0 ? Math.Round(skills[i].cdInfo.currentCD, 2).ToString() : "";
                characterUi.skillsUi.skills[i].skillFillamount.fillAmount = skills[i].cdInfo.currentCD / skills[i].cdInfo.cd;
            }
            else
            {
                characterUi.skillsUi.skills[i].skillTimer.text = "";
                characterUi.skillsUi.skills[i].skillFillamount.fillAmount = 0;
            }
        }
    }
    public void UpdateStatusEffect(ManagementStatusEffect.StatusEffectsData statusEffectsData)
    {
        if (!characterUi.statusEffectsUi.statusEffectsData.ContainsKey(statusEffectsData.statusEffectSO.typeStatusEffect))
        {
            StatusEffectUiHelper statusEffectsUi = Instantiate(Resources.Load<GameObject>("Prefabs/UI/StatusEffect/StatusEffectUi"), characterUi.statusEffectsUi.statusEffectContainer).GetComponent<StatusEffectUiHelper>();
            characterUi.statusEffectsUi.statusEffectsData.Add(
                statusEffectsData.statusEffectSO.typeStatusEffect,
                new StatusEffectsData(
                    statusEffectsUi,
                    statusEffectsData
                )
            );
        }
    }
    public void DestroyStatusEffect(StatusEffectSO.TypeStatusEffect typeStatusEffect)
    {
        if (characterUi.statusEffectsUi.statusEffectsData.TryGetValue(typeStatusEffect, out StatusEffectsData statusEffectsData))
        {
            Destroy(statusEffectsData.statusEffectUi.gameObject);
            characterUi.statusEffectsUi.statusEffectsData.Remove(typeStatusEffect);
        }
    }
    public void ToggleShowStatistics(bool isOpen)
    {
        characterUi.statisticsUi.mStatistics.SetBool("IsOpen", isOpen);
        IncreaseInventoryElapsedTime();
    }
    public void ToggleShowInventory(bool isOpen)
    {
        characterUi.inventoryUi.inventoryAnimator.SetBool("IsOpen", isOpen);
    }
    public void IncreaseInventoryElapsedTime()
    {
        characterUi.inventoryUi.elapsedTime = 5;
    }
    [Serializable]
    public class CharacterUi
    {
        public GameObject generalHud;
        public HudUi hudUi;
        public InteractUi interactUi;
        public ObjectsUi objectsUi;
        public SkillsUi skillsUi;
        public StatisticsUi statisticsUi;
        public StatusEffectsUi statusEffectsUi;
        public InformationMessageUi informationMessageUi;
        public GameObject[] mobileHud;
        public CharacterUi(HudUi hudUi, ObjectsUi objectsUi, SkillsUi skillsUi, StatisticsUi statisticsUi, StatusEffectsUi statusEffectsUi, InformationMessageUi informationMessageUi)
        {
            this.hudUi = hudUi;
            this.objectsUi = objectsUi;
            this.skillsUi = skillsUi;
            this.statisticsUi = statisticsUi;
            this.statusEffectsUi = statusEffectsUi;
            this.informationMessageUi = informationMessageUi;
        }
        public Image windUpFillAmount;
        public InventoryUi inventoryUi;
    }
    [Serializable]
    public class HudUi
    {
        public Image healthBarBack;
        public Image healthBarFront;
        public TMP_Text healthText;
        public Image manaBarBack;
        public Image manaBarFront;
        public TMP_Text manaText;
        public Image staminaBarBack;
        public Image staminaBarFront;
        public TMP_Text staminaText;
    }
    [Serializable]
    public class InteractUi
    {
        public GameObject bannerInteract;
        public RectTransform containerInteract;
        public RectTransform viewportInteract;
        public ScrollRect interactScrollRect;
        public List<ObjectsForTake> interacts = new List<ObjectsForTake>();
    }
    [Serializable]
    public class ObjectsUi
    {
        public ObjectsData[] objects = new ObjectsData[6];
        public Image containerObjects;
        public Color containerObjectsSecondaryActionColor;
    }
    [Serializable]
    public class ObjectsForTake
    {
        public GameObject interact;
        public BannerInteract bannerInteract;
    }
    [Serializable]
    public class ObjectsData
    {
        public Image objectBackground;
        public Image spriteObject;
        public Image usingObjectSprite;
        public TMP_Text amount;
    }
    [Serializable]
    public class SkillsUi
    {
        public SkillsData[] skills = new SkillsData[4];
        public Color highlightColor;
    }
    [Serializable]
    public class SkillsData
    {
        public Image skillBackground;
        public Image skillSprite;
        public Image skillFillamount;
        public TMP_Text skillTimer;
        public bool canActiveSkillSecondarySprite;

    }
    [Serializable]
    public class StatisticsUi
    {
        public Animator mStatistics;
        public StatisticsData[] statistics = new StatisticsData[]{
            new StatisticsData(null, null, Character.TypeStatistics.Atk),
            new StatisticsData(null, null, Character.TypeStatistics.Int),
            new StatisticsData(null, null, Character.TypeStatistics.AtkSpd),
            new StatisticsData(null, null, Character.TypeStatistics.Def),
            new StatisticsData(null, null, Character.TypeStatistics.Res),
            new StatisticsData(null, null, Character.TypeStatistics.Spd),
            new StatisticsData(null, null, Character.TypeStatistics.Crit),
            new StatisticsData(null, null, Character.TypeStatistics.CritDmg),
        };
        public StatisticsUi(Animator mStatistics, StatisticsData[] statistics)
        {
            this.mStatistics = mStatistics;
            this.statistics = statistics;
        }
    }
    [Serializable]
    public class StatisticsData
    {
        public Image sprite;
        public TMP_Text amount;
        public Character.TypeStatistics typeStatistic;

        public StatisticsData(Image sprite, TMP_Text amount, Character.TypeStatistics typeStatistic)
        {
            this.sprite = sprite;
            this.amount = amount;
            this.typeStatistic = typeStatistic;
        }
    }
    [Serializable]
    public class InformationMessageUi
    {
        public GameObject containerInformationMessage;
    }
    [Serializable]
    public class StatusEffectsUi
    {
        public Transform statusEffectContainer;
        public SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData> statusEffectsData = new SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData>();
    }
    [Serializable]
    public class StatusEffectsData
    {
        public StatusEffectUiHelper statusEffectUi;
        public ManagementStatusEffect.StatusEffectsData statusEffectsData;

        public StatusEffectsData(StatusEffectUiHelper statusEffectUi, ManagementStatusEffect.StatusEffectsData statusEffectsData)
        {
            this.statusEffectUi = statusEffectUi;
            this.statusEffectsData = statusEffectsData;
        }
    }
    [Serializable]
    public class InventoryUi
    {
        public Animator inventoryAnimator;
        public float elapsedTime = 0;
    }
}
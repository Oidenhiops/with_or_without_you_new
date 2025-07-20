using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManagementCharacterSkills : MonoBehaviour
{
    public PlayerInputs playerInputs;
    [SerializeField] Character character;
    [SerializeField] ManagementCharacterHud managementCharacterHud;
    [SerializeField] ManagementCharacterAnimations managementCharacterAnimations;
    public SkillInfo[] currentSkills = new SkillInfo[4];
    public int currentSkillIndex = 0;
    bool usingSkill;
    public void InitializeSkillsEvents()
    {
        if (playerInputs) playerInputs.characterActions.CharacterInputs.UseSkill.performed += OnUseSkill;
    }
    void OnDestroy()
    {
        if (playerInputs) playerInputs.characterActions.CharacterInputs.UseSkill.performed -= OnUseSkill;
    }
    public void Update()
    {
        if (character.isActive && GameManager.Instance.startGame)
        {
            if (currentSkills[0].skillData != null)
            {
                foreach (SkillInfo skill in currentSkills)
                {
                    if (skill.skillData != null && skill.cdInfo.currentCD > 0)
                    {
                        skill.cdInfo.currentCD -= Time.deltaTime;
                    }
                }
                managementCharacterHud.RefreshSkillsTimer(currentSkills);
            }
            if (usingSkill)
            {
                UseSkill();
            }
        }
    }
    void OnUseSkill(InputAction.CallbackContext context){
        if (character.isActive && playerInputs.characterActionsInfo.isSkillsActive && context.action.triggered)
        {
            currentSkillIndex = (int)context.ReadValue<float>();
            ValidateUseSkill();
        }
    }
    public void OnUseSkillMobile(int posSkill)
    {
        if (character.isActive)
        {
            currentSkillIndex = posSkill;
            ValidateUseSkill();
        }
    }
    void ValidateUseSkill()
    {
        if (currentSkills[currentSkillIndex].skillData != null && currentSkills[currentSkillIndex].cdInfo.currentCD <= 0)
        {
            if (currentSkills[currentSkillIndex].skillData.isPorcent)
            {
                int value = currentSkills[currentSkillIndex].skillData.isFromBaseValue ?
                    (int)Mathf.Ceil(character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).baseValue) :
                    (int)Mathf.Ceil(character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue);
                int amount = (int)(value * currentSkills[currentSkillIndex].skillData.cost.baseValue / 100);

                if (currentSkills[currentSkillIndex].skillData.cost.typeStatistics == Character.TypeStatistics.Hp &&
                    character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount > 1 ||
                    character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount >= 0)
                {
                    character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                    currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                    InitializeUsingSkill();
                }
            }
            else if (currentSkills[currentSkillIndex].skillData.cost.typeStatistics == Character.TypeStatistics.Hp &&
                     character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - currentSkills[currentSkillIndex].skillData.cost.baseValue >= 1 ||
                     character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - currentSkills[currentSkillIndex].skillData.cost.baseValue >= 0)
            {
                int amount = (int)Mathf.Ceil(currentSkills[currentSkillIndex].skillData.cost.baseValue);
                character.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                InitializeUsingSkill();
            }
        }
    }
    void InitializeUsingSkill()
    {
        if (currentSkills[currentSkillIndex].skillData.needAnimation)
        {
            managementCharacterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.Skill, currentSkills[currentSkillIndex].skillData.skillAnimation.animationName);
        }
        usingSkill = true;
    }
    void UseSkill()
    {
        if (!currentSkills[currentSkillIndex].skillData.needAnimation)
        {
            usingSkill = false;
        }
        else
        {
            if (managementCharacterAnimations.currentAnimation.typeAnimation == CharacterAnimationsSO.TypeAnimation.Skill)
            {
                if (managementCharacterAnimations.currentAnimation.frameToInstance == managementCharacterAnimations.characterAnimationsInfo.currentSpriteIndex && 
                    managementCharacterAnimations.currentAnimation.needInstance)
                {
                    usingSkill = false;
                    GameObject skill = Instantiate(currentSkills[currentSkillIndex].skillData.skillObject, transform.position, Quaternion.identity, transform);
                    skill.GetComponent<ISkill>().SendInformation(character.characterStatistics, character);
                }
            }
            else
            {
                usingSkill = false;
            }
        }
    }
    public void InitializeSkills()
    {
        if (GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null)
        {
            currentSkills = GameData.Instance.saveData.gameInfo.characterInfo.currentSkills;
        }
        for (int i = 0; i < currentSkills.Length; i++)
        {
            if (currentSkills[i].skillData != null)
            {
                currentSkills[i].cdInfo = new SkillDataScriptableObject.CdInfo
                {
                    cd = currentSkills[i].skillData.cdInfo.cd
                };
            }
        }
        managementCharacterHud.RefreshSkillsSprites(currentSkills);
    }
    [Serializable] public class SkillInfo
    {
        public int skillId;
        public SkillDataScriptableObject skillData;
        public SkillDataScriptableObject.CdInfo cdInfo = new SkillDataScriptableObject.CdInfo();
    }
    public interface ISkill
    {
        public void SendInformation(Dictionary<Character.TypeStatistics, Character.Statistics> statistics, Character character);
    }
}

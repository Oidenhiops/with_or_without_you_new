using System;
using System.Collections;
using UnityEngine;

public class ManagementCharacterAttack : MonoBehaviour
{
    [SerializeField] protected Character character;
    [SerializeField] GameObject posisionAttack;
    ManagementCharacterModelDirection.ICharacterDirection characterDirection;
    float distAttack = 1;
    public float distLostTarget = 1;
    protected float cooldownAttack = 0;
    protected Character.Statistics costsAttack = new Character.Statistics(Character.TypeStatistics.Sp, 10, 0, 0, 0, 0);
    void Start()
    {
        characterDirection = GetComponent<ManagementCharacterModelDirection.ICharacterDirection>();
    }
    void Update()
    {
        if (cooldownAttack > 0)
        {
            cooldownAttack -= Time.deltaTime;
        }
    }
    public virtual void ValidateAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distAttack, LayerMask.GetMask("Player"));
        if (hitColliders.Length > 0 && hitColliders[0].GetComponent<Character>().characterInfo.isActive)
        {
            if (character.characterInfo.characterScripts.characterAnimations != null &&
                character.characterInfo.characterScripts.characterAnimations.ValidateAnimationEnd("TakeDamage") &&
                ValidateAllAnimationsAttackEnd() && cooldownAttack <= 0)
            {
                characterDirection.SetTarget(hitColliders[0].gameObject);
                StartAttack();
            }
        }
    }
    public virtual void ValidateAttackMobile() { Debug.LogError("ValidateAttackMobile not implemented"); }
    protected void StartAttack()
    {        
        character.characterInfo.characterScripts.characterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.Attack, ValidateTypeAttack());
        SetCoolDown();
        Attack();
    }
    string ValidateTypeAttack()
    {
        foreach(ManagementCharacterObjects.ObjectsInfo weapon in character.characterInfo.characterScripts.managementCharacterObjects.objects)
        {
            if (weapon.objectData && weapon.isUsingItem)
            {
                if (weapon.objectData.objectInstance.TryGetComponent<ManagementWeaponsObject>(out ManagementWeaponsObject weaponObject))
                {
                    return weaponObject.typeAnimation;
                }
            }
        }
        return "GeneralAttack";
    }
    protected bool ValidateAllAnimationsAttackEnd()
    {
        if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != CharacterAnimationsSO.TypeAnimation.Attack && 
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != CharacterAnimationsSO.TypeAnimation.Skill)
            {
                return true;
            }
        return false;
    }
    void SetCoolDown()
    {
        cooldownAttack = 1 / character.characterInfo.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
    }
    void Attack()
    {
        StartCoroutine(WaitToAttack());
    }
    IEnumerator WaitToAttack()
    {
        while (true)
        {
            CharacterAnimationsSO.AnimationsInfo currentAnimation = character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation();
            CharacterAnimationsSO.CharacterAnimationsInfo currentAnimationInfo = character.characterInfo.characterScripts.characterAnimations.GetAnimationsInfo();
            if (currentAnimationInfo.currentSpriteIndex >= currentAnimation.frameToInstance && currentAnimation.needInstance)
            {
                GameObject instance = Instantiate(character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().instanceObj, transform.position, Quaternion.identity);
                instance.layer = character.characterInfo.isPlayer ? LayerMask.NameToLayer("PlayerAttack") : LayerMask.NameToLayer("EnemyAttack");
                instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetDamage(character.characterInfo.GetStatisticByType(Character.TypeStatistics.Atk).currentValue);
                instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetObjectMakeDamage(character);
                instance.GetComponent<ManagementCharacterInstance>().SetInfoForAnimation(character.characterInfo.characterScripts.managementCharacterModelDirection.GetDirectionAnimation(), character.characterInfo.characterScripts.characterAnimations.GetAnimationsInfo());
                instance.transform.position = character.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.position;
                instance.transform.rotation = character.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.rotation;
                instance.transform.localScale = Vector3.one;
                AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("Slash"), 1, true);
                break;
            }
            if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().animationName == "TakeDamage")
            {
                StopAllCoroutines();
            }
            yield return null;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distAttack);
    }
    public float GetDistLostTarget()
    {
        return distLostTarget;
    }
    public GameObject GetDirectionAttack()
    {
        return posisionAttack;
    }
}

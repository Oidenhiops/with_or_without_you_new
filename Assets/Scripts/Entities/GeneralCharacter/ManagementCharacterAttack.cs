using System;
using System.Collections;
using UnityEngine;

public class ManagementCharacterAttack : MonoBehaviour
{
    [SerializeField] protected Character character;
    [SerializeField] GameObject posisionAttack;
    PlayerModelDirection characterDirection;
    float distAttack = 1;
    public float distLostTarget = 1;
    public float cooldownAttack = 0;
    protected Character.Statistics costsAttack = new Character.Statistics(Character.TypeStatistics.Sp, 10, 0, 0, 0, 0);
    void Start()
    {
        characterDirection = GetComponent<PlayerModelDirection>();
    }
    void Update()
    {
        if (character.isActive && GameManager.Instance.startGame)
        {
            ValidateAttack();
        }
        if (cooldownAttack > 0)
        {
            cooldownAttack -= Time.deltaTime;
            if (cooldownAttack <= 0) cooldownAttack = 0;
        }
    }
    public virtual void ValidateAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distAttack, LayerMask.GetMask("Player"));
        if (hitColliders.Length > 0 && hitColliders[0].GetComponent<Character>().isActive)
        {
            if (character.characterAnimations != null &&
                character.characterAnimations.ValidateAnimationEnd("TakeDamage") &&
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
        character.characterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.Attack, ValidateTypeAttack());
        SetCoolDown();
        Attack();
    }
    string ValidateTypeAttack()
    {
        foreach(ManagementCharacterObjects.ObjectsInfo weapon in character.characterObjects.objects)
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
        if (character.characterAnimations.GetCurrentAnimation().typeAnimation != CharacterAnimationsSO.TypeAnimation.Attack && 
            character.characterAnimations.GetCurrentAnimation().typeAnimation != CharacterAnimationsSO.TypeAnimation.Skill)
            {
                return true;
            }
        return false;
    }
    void SetCoolDown()
    {
        cooldownAttack = 1 / character.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
    }
    void Attack()
    {
        StartCoroutine(WaitToAttack());
    }
    IEnumerator WaitToAttack()
    {
        while (true)
        {
            CharacterAnimationsSO.AnimationsInfo currentAnimation = character.characterAnimations.GetCurrentAnimation();
            CharacterAnimationsSO.CharacterAnimationsInfo currentAnimationInfo = character.characterAnimations.GetAnimationsInfo();
            if (currentAnimationInfo.currentSpriteIndex >= currentAnimation.frameToInstance && currentAnimation.needInstance)
            {
                GameObject instance = Instantiate(character.characterAnimations.GetCurrentAnimation().instanceObj, transform.position, Quaternion.identity);
                instance.layer = character.isPlayer ? LayerMask.NameToLayer("PlayerAttack") : LayerMask.NameToLayer("EnemyAttack");
                instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetDamage(character.GetStatisticByType(Character.TypeStatistics.Atk).currentValue);
                instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetObjectMakeDamage(character);
                instance.GetComponent<ManagementCharacterInstance>().SetInfoForAnimation(character.characterModelDirection.movementDirectionAnimation, character.characterAnimations.GetAnimationsInfo());
                instance.transform.position = character.characterAttack.GetDirectionAttack().transform.position;
                instance.transform.rotation = character.characterAttack.GetDirectionAttack().transform.rotation;
                instance.transform.localScale = Vector3.one;
                AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("Slash"), 1, true);
                break;
            }
            if (character.characterAnimations.GetCurrentAnimation().animationName == "TakeDamage")
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

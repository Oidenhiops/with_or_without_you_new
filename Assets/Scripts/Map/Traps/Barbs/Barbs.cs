using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbs : MonoBehaviour
{
    public Animator animator;
    public float damage = 0;
    public StatusEffectSO[] statusEffects;
    public float timeHitStop;
    public float timeToRestoreCharacterToHit = 0.1f;
    public List<Character> charactersHited = new List<Character>();
    void LateUpdate()
    {
        if (charactersHited.Count == 0)
        {
            animator.SetBool("isActive", false);
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            Character character = other.GetComponent<Character>();
            if (!charactersHited.Contains(character))
            {
                charactersHited.Add(character);
                int damageToMake = (int)MathF.Round(character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue * damage / 100);
                character.characterInfo.TakeDamage(damageToMake, Color.white, timeHitStop, Character.TypeDamage.TrueDamage, null);
                foreach (StatusEffectSO statusEffect in statusEffects)
                {
                    character.characterInfo.characterScripts.managementStatusEffect.AddStatus
                    (
                        statusEffect,
                        gameObject,
                        other.gameObject
                    );
                }
                StartCoroutine(RestoreCharacterToHit(character));
            }
            if (charactersHited.Count > 0)
            {
                animator.SetBool("isActive", true);
            }
        }
    }
    public IEnumerator RestoreCharacterToHit(Character characterToRestore)
    {
        yield return new WaitForSeconds(timeToRestoreCharacterToHit);
        if (charactersHited.Contains(characterToRestore))
        {
            charactersHited.Remove(characterToRestore);
        }
    }
}

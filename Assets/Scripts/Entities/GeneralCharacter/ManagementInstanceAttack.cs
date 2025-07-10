using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagementInstanceAttack : MonoBehaviour, ManagementInstanceAttack.IInstanceAttack
{
    public InstanceAttackInfo instanceAttackInfo = new InstanceAttackInfo();

    public void SetDamage(float value)
    {
        instanceAttackInfo.damage = Mathf.CeilToInt(value);
    }

    public void SetObjectMakeDamage(Character characterMakeDamage)
    {
        instanceAttackInfo.characterMakeDamage = characterMakeDamage;
    }

    [Serializable]  public class InstanceAttackInfo 
    {
        public Character.TypeDamage typeDamage;
        public float damage = 0;
        public bool isPorcent = false;
        public Color colorDamage = Color.white;
        public Character characterMakeDamage;
        public bool canDestroy = false;
        public StatusEffectSO[] statusEffects;
        public float timeHitStop;
        public bool isMultipleAttack;
        public float timeToRestoreCharacterToHit = 0.1f;        
        public List<Character> charactersHited = new List<Character>();
    }
    public interface IInstanceAttack
    {
        public void SetDamage(float value);
        public void SetObjectMakeDamage(Character characterMakeDamage);
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            Character character = other.GetComponent<Character>();
            if (!instanceAttackInfo.charactersHited.Contains(character))
            {
                instanceAttackInfo.charactersHited.Add(character);
                int damage = !instanceAttackInfo.isPorcent ? (int)instanceAttackInfo.damage : (int)MathF.Round(character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue * instanceAttackInfo.damage / 100);
                character.characterInfo.TakeDamage(damage, instanceAttackInfo.colorDamage, instanceAttackInfo.timeHitStop, instanceAttackInfo.typeDamage, instanceAttackInfo.characterMakeDamage);
                foreach (StatusEffectSO statusEffect in instanceAttackInfo.statusEffects)
                {
                    character.characterInfo.characterScripts.managementStatusEffect.AddStatus
                    (
                        statusEffect,
                        instanceAttackInfo.characterMakeDamage.gameObject,
                        other.gameObject
                    );
                }
                if (instanceAttackInfo.isMultipleAttack)
                {
                    StartCoroutine(RestoreCharacterToHit(character));
                }
            }
        }
        else if (instanceAttackInfo.canDestroy)
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator RestoreCharacterToHit(Character characterToRestore)
    {
        yield return new WaitForSeconds(instanceAttackInfo.timeToRestoreCharacterToHit);
        if (instanceAttackInfo.charactersHited.Contains(characterToRestore))
        {
            instanceAttackInfo.charactersHited.Remove(characterToRestore);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagementSlashSkill : MonoBehaviour, ManagementCharacterSkills.ISkill
{
    public List<Character.Statistics> baseStatistics = new List<Character.Statistics>();
    public List<Character.Statistics> currentStatistics = new List<Character.Statistics>();
    public Character characterMakeDamage;
    public Rigidbody rb;
    public float speed;
    public void SendInformation(Dictionary<Character.TypeStatistics, Character.Statistics> statistics, Character character)
    {
        gameObject.layer = LayerMask.NameToLayer(character.characterInfo.isPlayer ? "PlayerAttack" : "EnemyAttack");
        characterMakeDamage = character;
        foreach(Character.Statistics statistic in baseStatistics)
        {
            currentStatistics.Add(new Character.Statistics(statistic.typeStatistics, 0,0,0,0,0));
        }
        for (int i = 0; i < currentStatistics.Count; i++)
        {
            int amount = (int)Mathf.Round(GetStatistics(currentStatistics[i].typeStatistics, character).currentValue * GetStatistics(currentStatistics[i].typeStatistics, baseStatistics).baseValue / 100);
            currentStatistics[i].baseValue = amount;
        }
        MakeSkill(character);
    }
    public Character.Statistics GetStatistics(Character.TypeStatistics typeStatistics, Character character)
    {
        return character.characterInfo.GetStatisticByType(typeStatistics);
    }
    public Character.Statistics GetStatistics(Character.TypeStatistics typeStatistics, List<Character.Statistics> statistics)
    {
        foreach (Character.Statistics statistic in statistics)
        {
            if (statistic.typeStatistics == typeStatistics)
            {
                return statistic;
            }
        }
        return null;
    }
    public void MakeSkill(Character character)
    {
        transform.position += characterMakeDamage.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.forward;
        transform.rotation = characterMakeDamage.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.rotation;
        transform.SetParent(null);
        rb.AddForce(characterMakeDamage.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.forward * speed);
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("Slash"), 1, true);
        if (TryGetComponent<ManagementInstanceAttack>(out ManagementInstanceAttack managementInstanceAttack))
        {
            managementInstanceAttack.instanceAttackInfo.characterMakeDamage = character;
            managementInstanceAttack.instanceAttackInfo.damage = GetStatistics(Character.TypeStatistics.Atk, currentStatistics).baseValue;
        }
        Destroy(rb, 2);
        Destroy(gameObject.GetComponent<Collider>(), 2);
        Destroy(gameObject, 6);
    }
}

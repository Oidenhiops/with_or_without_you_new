using System.Collections.Generic;
using UnityEngine;

public class ManagementTripleSlashSkill : MonoBehaviour, ManagementCharacterSkills.ISkill
{
    public List<GameObject> slashes;
    public List<Character.Statistics> baseStatistics = new List<Character.Statistics>();
    public List<Character.Statistics> currentStatistics = new List<Character.Statistics>();
    public Character characterMakeDamage;
    public float speed;
    public void SendInformation(Dictionary<Character.TypeStatistics, Character.Statistics> statistics, Character character)
    {
        characterMakeDamage = character;
        foreach (Character.Statistics statistic in baseStatistics)
        {
            currentStatistics.Add(new Character.Statistics(statistic.typeStatistics, 0, 0, 0, 0, 0));
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
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("Slash"), 1, true);
        for (int i = 0; i < slashes.Count; i++)
        {
            slashes[i].layer = LayerMask.NameToLayer(character.characterInfo.isPlayer ? "PlayerAttack" : "EnemyAttack");
            Rigidbody rb = slashes[i].GetComponent<Rigidbody>();
            slashes[i].transform.SetParent(null);
            rb.AddForce(slashes[i].transform.forward * speed);
            slashes[i].GetComponent<ManagementInstanceAttack>().instanceAttackInfo.characterMakeDamage = character;
            slashes[i].GetComponent<ManagementInstanceAttack>().instanceAttackInfo.damage = GetStatistics(Character.TypeStatistics.Atk, currentStatistics).baseValue;
            Destroy(rb, 2);
            Destroy(slashes[i].GetComponent<Collider>(), 2);
            Destroy(slashes[i], 6);
        }
        Destroy(gameObject);
    }
}

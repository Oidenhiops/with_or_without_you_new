using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsDB", menuName = "ScriptableObjects/DB/SkillsDB", order = 1)]
public class SkillsDBSO : ScriptableObject
{
    public SerializedDictionary<int, SkillDataScriptableObject> skills;

    public SkillDataScriptableObject GetSkill(int id)
    {
        if (skills.TryGetValue(id, out SkillDataScriptableObject skillsDataSO))
        {
            return skillsDataSO;
        }
        return null;
    }
}
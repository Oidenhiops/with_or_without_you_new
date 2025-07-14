using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialData", menuName = "ScriptableObjects/Character/InitialDataSO", order = 1)]
public class InitialDataSO : ScriptableObject
{
    public bool isHuman = false;
    public Texture2D atlas;
    public Texture2D atlasHand;
    public SerializedDictionary<Character.TypeStatistics, Character.Statistics> characterStatistics = new SerializedDictionary<Character.TypeStatistics, Character.Statistics>{
        {Character.TypeStatistics.Hp, new Character.Statistics (Character.TypeStatistics.Hp, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Sp, new Character.Statistics (Character.TypeStatistics.Sp, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Mp, new Character.Statistics (Character.TypeStatistics.Mp, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Atk, new Character.Statistics (Character.TypeStatistics.Atk, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.AtkSpd, new Character.Statistics (Character.TypeStatistics.AtkSpd, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Int, new Character.Statistics (Character.TypeStatistics.Int, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Def, new Character.Statistics (Character.TypeStatistics.Def, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Res, new Character.Statistics (Character.TypeStatistics.Res, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Spd, new Character.Statistics (Character.TypeStatistics.Spd, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.Crit, new Character.Statistics (Character.TypeStatistics.Crit, 0, 0, 0, 0, 0)},
        {Character.TypeStatistics.CritDmg, new Character.Statistics (Character.TypeStatistics.CritDmg, 0, 0, 0, 0, 0)},
    };
    public ManagementCharacterSkills.SkillInfo[] skills;
    public ManagementCharacterObjects.ObjectsInfo[] objects = new ManagementCharacterObjects.ObjectsInfo[0];
    public CharacterAnimationsSO characterAnimations;
    public InitialDataSO Clone()
    {
        InitialDataSO clone = ScriptableObject.CreateInstance<InitialDataSO>();

        clone.isHuman = this.isHuman;
        clone.atlas = this.atlas;
        clone.atlasHand = this.atlasHand;
        clone.characterStatistics = this.characterStatistics;
        clone.skills = this.skills;

        clone.name = clone.atlas.name;

        clone.objects = new ManagementCharacterObjects.ObjectsInfo[this.objects.Length];
        for (int i = 0; i < this.objects.Length; i++)
        {
            clone.objects[i] = this.objects[i];
        }

        clone.characterAnimations = Instantiate(this.characterAnimations);

        return clone;
    }
}
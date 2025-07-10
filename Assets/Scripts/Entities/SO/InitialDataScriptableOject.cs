using UnityEngine;

[CreateAssetMenu(fileName = "InitialData", menuName = "ScriptableObjects/Character/InitialDataSO", order = 1)]
public class InitialDataSO : ScriptableObject
{
    public bool isHuman = false;
    public Texture2D atlas;
    public Texture2D atlasHand;
    public Character.CharacterInfo characterInfo = new Character.CharacterInfo();
    public ManagementCharacterSkills.SkillInfo[] skills;
    public ManagementCharacterObjects.ObjectsInfo[] objects = new ManagementCharacterObjects.ObjectsInfo[0];
    public CharacterAnimationsSO characterAnimations;
    public InitialDataSO Clone()
    {
        InitialDataSO clone = ScriptableObject.CreateInstance<InitialDataSO>();

        clone.isHuman = this.isHuman;
        clone.atlas = this.atlas;
        clone.atlasHand = this.atlasHand;
        clone.characterInfo = this.characterInfo;
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
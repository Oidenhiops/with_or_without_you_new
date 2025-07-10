using UnityEngine;
[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/SkillsDataScriptableObject", order = 1)]
public class SkillDataScriptableObject : ScriptableObject
{
    public int skillId = 0;
    public GameObject skillObject;
    public Sprite skillSprite;
    public Character.Statistics cost;
    public bool isPorcent = false;
    public bool isFromBaseValue = false;
    public bool needAnimation = false;
    public CharacterAnimationsSO.AnimationsInfo skillAnimation = new CharacterAnimationsSO.AnimationsInfo();
    public CdInfo cdInfo = new CdInfo();
    [System.Serializable]
    public class CdInfo
    {
        public float cd = 0;
        public float currentCD = 0;
    }
    public int textId = 0;
}

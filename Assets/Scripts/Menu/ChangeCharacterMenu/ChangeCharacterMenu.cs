using System.Linq;
using System.Text.RegularExpressions;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCharacterMenu : MonoBehaviour
{
    public StaticticsUi[] staticticsUi;
    public InitialDataSO characterSelected;
    public SerializedDictionary<int, CharactersDBSO.CharactersData> allCharacters = new SerializedDictionary<int, CharactersDBSO.CharactersData>();
    public bool isSkillWindow = false;
    public bool isObjectsWindow = false;
    public TMP_Text nameCharacter;
    public int currentIndex = 0;
    public Character character;
    public GameObject containerSkills;
    public GameObject containerObjects;
    public Button startButton;
    public Button playButton;

    public void OnEnable()
    {
        foreach (var kv in GameData.Instance.charactersDBSO.characters)
        {
            if (kv.Value.isUnlock)
            {
                allCharacters.Add(kv.Key, kv.Value);
            }
        }
        characterSelected = GameData.Instance.saveData.gameInfo.characterInfo.characterSelected.initialDataSO;
        GameData.Instance.saveData.gameInfo.characterInfo.currentSkills = characterSelected.skills;
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (characterSelected == allCharacters[i].initialDataSO)
            {
                currentIndex = i;
                break;
            }
        }
        SetCharacterData();
        startButton.Select();
    }
    void OnDisable()
    {
        playButton.Select();
    }
    public void SetCharacterData()
    {
        for (int u = 0; u < staticticsUi.Length; u++)
        {
            for (int s = 0; s < characterSelected.characterStatistics.Count; s++)
            {
                if (staticticsUi[u].typeStatistics == characterSelected.characterStatistics.ElementAt(s).Value.typeStatistics)
                {
                    staticticsUi[u].statisticsText.text = characterSelected.characterStatistics.ElementAt(s).Value.baseValue.ToString();
                }
            }
        }
        nameCharacter.text = Regex.Replace(characterSelected.name, "(?<=\\p{Ll})(\\p{Lu})", " $1");
        SetSkillsData();
        SetObjectsData();
    }
    public void SetSkillsData()
    {
        for (int i = containerSkills.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(containerSkills.transform.GetChild(i).gameObject);
        }
        GameData.Instance.saveData.gameInfo.characterInfo.currentSkills = new ManagementCharacterSkills.SkillInfo[4];
        for (int i = 0; i < characterSelected.skills.Length; i++)
        {
            GameData.Instance.saveData.gameInfo.characterInfo.currentSkills[i] = new ManagementCharacterSkills.SkillInfo
            {
                skillId = characterSelected.skills[i].skillData.skillId,
                skillData = characterSelected.skills[i].skillData,
                cdInfo = new SkillDataScriptableObject.CdInfo(),
            };
            GameObject characterSkillInstance = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Menu/ChangeCharacterSkill/SkillBackground"), containerSkills.transform);
            CharacterSkillMenu characterSkill = characterSkillInstance.GetComponent<CharacterSkillMenu>();
            characterSkill.SetSkillData(characterSelected.skills[i].skillData.skillSprite);
        }
    }
    public void SetObjectsData()
    {
        for (int i = containerObjects.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(containerObjects.transform.GetChild(i).gameObject);
        }
        GameData.Instance.saveData.gameInfo.characterInfo.currentObjects = new ManagementCharacterObjects.ObjectsInfo[6];
        for (int i = 0; i < characterSelected.objects.Length; i++)
        {
            GameData.Instance.saveData.gameInfo.characterInfo.currentObjects[i] = new ManagementCharacterObjects.ObjectsInfo{
                objectPos = characterSelected.objects[i].objectPos,
                objectId = characterSelected.objects[i].objectData.objectId,
                objectData = characterSelected.objects[i].objectData,
                amount = characterSelected.objects[i].amount,
                isUsingItem = characterSelected.objects[i].isUsingItem,
            };
            GameObject characterObjectInstance = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Menu/ChangeCharacterObjects/ObjectBackground"), containerObjects.transform);
            ManagementChangeCharacterObject characterObject = characterObjectInstance.GetComponent<ManagementChangeCharacterObject>();
            characterObject.SetObjectData(characterSelected.objects[i].objectData.objectSprite, characterSelected.objects[i].amount);
        }
    }
    public void ChangeCharacter(bool direction)
    {
        if (direction)
        {
            currentIndex++;
        }
        else
        {
            currentIndex--;
        }
        if (currentIndex > allCharacters.Count - 1)
        {
            currentIndex = 0;
        }
        else if (currentIndex < 0)
        {
            currentIndex = allCharacters.Count - 1;
        }
        int key = allCharacters.ToList()[currentIndex].Key;
        characterSelected = allCharacters[key].initialDataSO;
        GameData.Instance.saveData.gameInfo.characterInfo.characterSelected = allCharacters[key];
        GameData.Instance.saveData.gameInfo.characterInfo.currentSkills = characterSelected.skills;
        GameData.Instance.saveData.gameInfo.characterInfo.characterSelectedId = key;
        SetCharacterSprite();
        SetCharacterData();
    }
    public void ChangeCurrentWindow(int window)
    {
        switch (window)
        {
            case 0:
                isObjectsWindow = false;
                break;
            case 1:
                isObjectsWindow = true;
                break;
            case 2:
                isObjectsWindow = true;
                break;
        }
    }
    public void SetCharacterSprite()
    {
        character.characterAnimations.SetInitialData(characterSelected);
    }

    [System.Serializable]
    public class StaticticsUi
    {
        public Character.TypeStatistics typeStatistics;
        public TMP_Text statisticsText;
    }
}

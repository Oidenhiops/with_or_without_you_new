using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }
    string nameSaveData = "SaveData.json";
    public ItemsDBSO itemsDB;
    public SkillsDBSO skillsDB;
    public SaveData saveData = new SaveData();
    public List<string[]> csvData = new List<string[]>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _= LoadData();
    }
    public async Awaitable LoadData()
    {
        try
        {
            CheckFileExistance(DataPath());
            saveData = ReadDataFromJson();
            saveData.gameInfo.characterInfo.characterSelected = Resources.Load<InitialDataSO>($"SciptablesObjects/Character/InitialData/{saveData.gameInfo.characterInfo.characterSelectedName}");
            LoadCSV();
            InitializeResolutionData();
            InitializeItems();
            InitializeSkills();
            Application.targetFrameRate = saveData.configurationsInfo.FpsLimit;
            await InitializeAudioMixerData();
            saveData.configurationsInfo.canShowFps = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Language/Language");
        string[] lines = csvFile.text.Split('\n');
        List<String[]> textData = new List<string[]>();
        foreach (string line in lines)
        {
            string[] columns = line.Split(';');
            textData.Add(columns);
        }
        csvData = textData;
    }
    public string GetDialog(int id)
    {
        int languageIndex = 0;
        for (int i = 0; i < csvData[0].Length; i++)
        {
            if (csvData[0][i] == saveData.configurationsInfo.currentLanguage.ToString())
            {
                languageIndex = i;
                break;
            }
        }
        if (languageIndex != 0) return csvData[id][languageIndex];
        return null;
    }
    public void ChangeLanguage(TypeLanguage language)
    {
        saveData.configurationsInfo.currentLanguage = language;
        SaveGameData();
    }
    void InitializeItems()
    {
        foreach(ManagementCharacterObjects.ObjectsInfo objectsDataSO in saveData.gameInfo.characterInfo.currentObjects)
        {
            objectsDataSO.objectData = itemsDB.GetItem(objectsDataSO.objectId);
        }
    }
    void InitializeSkills()
    {
        foreach(ManagementCharacterSkills.SkillInfo skillDataSO in saveData.gameInfo.characterInfo.currentSkills)
        {
            skillDataSO.skillData = skillsDB.GetSkill(skillDataSO.skillId);
        }
    }
    void InitializeResolutionData()
    {
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC)
        {
            Screen.SetResolution(
                saveData.configurationsInfo.resolutionConfiguration.currentResolution.width,
                saveData.configurationsInfo.resolutionConfiguration.currentResolution.height,
                saveData.configurationsInfo.resolutionConfiguration.isFullScreen
            );
        }
        else
        {
            Screen.SetResolution(
                Screen.width,
                Screen.height,
                true
            );
        }
    }
    async Awaitable InitializeAudioMixerData()
    {
        try
        {
            await Awaitable.NextFrameAsync();
            float decibelsBGM = 20 * Mathf.Log10(saveData.configurationsInfo.soundConfiguration.BGMalue / 100);
            float decibelsSFX = 20 * Mathf.Log10(saveData.configurationsInfo.soundConfiguration.SFXalue / 100);
            if (saveData.configurationsInfo.soundConfiguration.BGMalue == 0) decibelsBGM = -80;
            if (saveData.configurationsInfo.soundConfiguration.SFXalue == 0) decibelsSFX = -80;
            AudioManager.Instance.audioMixer.SetFloat(AudioManager.TypeSound.BGM.ToString(), decibelsBGM);
            AudioManager.Instance.audioMixer.SetFloat(AudioManager.TypeSound.SFX.ToString(), decibelsSFX);
            if (saveData.configurationsInfo.soundConfiguration.isMute)
            {
                AudioManager.Instance.audioMixer.SetFloat(AudioManager.TypeSound.Master.ToString(), -80f);
            }
            else
            {
                GameManager.Instance.StartCoroutine(AudioManager.Instance.FadeIn());
            }
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    public void SetStartingData()
    {        
        SaveData dataInfo = new SaveData();
        dataInfo.configurationsInfo.currentLanguage = TypeLanguage.English;
        SetStartingDataSound(dataInfo);
        SetStartingPlayerData(dataInfo);
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC) SetStartingResolution(ref dataInfo);        
        saveData.gameInfo = new GameInfo();
        saveData = dataInfo;
        SaveGameData();
    }
    void SetStartingPlayerData(SaveData dataInfo)
    {
        CharacterInfo newCharacterInfo = new CharacterInfo();
        newCharacterInfo.characterSelectedName = "Warrior";
        dataInfo.gameInfo.characterInfo = newCharacterInfo;
    }
    void SetStartingDataSound(SaveData dataInfo)
    {
        dataInfo.configurationsInfo.soundConfiguration.MASTERValue = 25;
        dataInfo.configurationsInfo.soundConfiguration.BGMalue = 25;
        dataInfo.configurationsInfo.soundConfiguration.SFXalue = 25;
    }
    void SetStartingResolution(ref SaveData dataInfo)
    {
        Resolution[] resolutions = Screen.resolutions;
        Array.Reverse(resolutions);
        foreach (Resolution res in resolutions)
        {
            dataInfo.configurationsInfo.resolutionConfiguration.allResolutions.Add(new ResolutionsInfo(res.width, res.height));
        }
        Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
        dataInfo.configurationsInfo.resolutionConfiguration.isFullScreen = true;
        dataInfo.configurationsInfo.resolutionConfiguration.currentResolution = new ResolutionsInfo(
            resolutions[0].width,
            resolutions[0].height);
    }
    public void SaveGameData()
    {
        WriteDataToJson();
    }
    void CheckFileExistance(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            SetStartingData();
            string dataString = JsonUtility.ToJson(saveData);
            File.WriteAllText(filePath, dataString);
        }
    }
    SaveData ReadDataFromJson()
    {
        string dataString;
        string jsonFilePath = DataPath();
        dataString = File.ReadAllText(jsonFilePath);
        saveData = JsonUtility.FromJson<SaveData>(dataString);
        return saveData;
    }
    public void WriteDataToJson()
    {
        try
        {
            string jsonFilePath = DataPath();
            string dataString = JsonUtility.ToJson(saveData);
            File.WriteAllText(jsonFilePath, dataString);
        }
        catch (Exception e)
        {
            print(e);
        }
    }
    string DataPath()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            return Path.Combine(Application.persistentDataPath, nameSaveData);
        }
        return Path.Combine(Application.streamingAssetsPath, nameSaveData);
    }
    [Serializable]  public class SaveData
    {
        public GameInfo gameInfo = new GameInfo();
        public ConfigurationsInfo configurationsInfo = new ConfigurationsInfo();
    }
    [Serializable]  public class GameInfo
    {
        public CharacterInfo characterInfo = new CharacterInfo();
    }
    [Serializable]  public class CharacterInfo
    {
        public bool isInitialize = false;
        public string characterSelectedName;
        public InitialDataSO characterSelected;
        public ManagementCharacterSkills.SkillInfo[] currentSkills = new ManagementCharacterSkills.SkillInfo[4];
        public ManagementCharacterObjects.ObjectsInfo[] currentObjects = new ManagementCharacterObjects.ObjectsInfo[6];
    }
    [Serializable] public class ConfigurationsInfo
    {
        public TypeLanguage _currentLanguage;
        public Action<TypeLanguage> OnLanguageChange;
        public TypeLanguage currentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnLanguageChange?.Invoke(_currentLanguage);
                }
            }
        }
        public bool _canShowFps;
        public Action<bool> OnCanShowFpsChange;
        public bool canShowFps
        {
            get => _canShowFps;
            set
            {
                if (_canShowFps != value)
                {
                    _canShowFps = value;
                    OnCanShowFpsChange?.Invoke(_canShowFps);
                }
            }
        }
        public int FpsLimit = 0;
        public ResolutionConfiguration resolutionConfiguration = new ResolutionConfiguration();
        public SoundConfiguration soundConfiguration = new SoundConfiguration();
    }
    [Serializable]  public class SoundConfiguration
    {
        public bool isMute = false;
        public float MASTERValue;
        public float BGMalue;
        public float SFXalue;
    }
    [Serializable]  public class ResolutionConfiguration
    {
        public bool isFullScreen = false;
        public List<ResolutionsInfo> allResolutions = new List<ResolutionsInfo>();
        public ResolutionsInfo currentResolution;
    }
    [Serializable]  public class ResolutionsInfo
    {
        public int width = 0;
        public int height = 0;
        public ResolutionsInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    public enum TypeLanguage
    {
        English = 0,
        Español = 1,
    }
}

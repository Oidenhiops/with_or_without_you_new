using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "CharactersDB", menuName = "ScriptableObjects/DB/CharactersDB", order = 1)]
public class CharactersDBSO : ScriptableObject
{
    public SerializedDictionary<int, CharactersData> characters;

    [System.Serializable]
    public class CharactersData
    {
        public InitialDataSO initialDataSO;
        public bool isUnlock = false;
    }
}
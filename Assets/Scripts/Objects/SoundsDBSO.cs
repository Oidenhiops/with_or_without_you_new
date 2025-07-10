using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsDB", menuName = "ScriptableObjects/DB/SoundsDB", order = 1)]
public class SoundsDBSO : ScriptableObject
{
    public SerializedDictionary<string, AudioClip[]> sounds;
}

using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsData", menuName = "ScriptableObjects/ObjectsDataSO", order = 1)]

public class ItemsDataSO : ScriptableObject
{
    public int objectId = 0;
    public TypeObject typeObject;
    public Character.Statistics[] statistics;
    public bool isPorcent = false;
    public bool canStack = false;
    public float maxStack = 0;
    public Sprite objectSprite;
    public GameObject objectInstance;
    public GameObject objectInstanceForUse;
    public Color colorEffect = Color.white;
    public AudioClip effectAudio;    
    public bool isOnlyForMonsters;
    public enum TypeObject
    {
        None = 0,
        Object = 1,
        Weapon = 2,
        Armor = 3,
        ObjectConsumable = 4
    }
}
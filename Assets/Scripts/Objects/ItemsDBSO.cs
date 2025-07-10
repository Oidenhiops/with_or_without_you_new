using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsDB", menuName = "ScriptableObjects/DB/ItemsDB", order = 1)]
public class ItemsDBSO : ScriptableObject
{
    public SerializedDictionary<int, ItemsDataSO> items;

    public ItemsDataSO GetItem(int id)
    {
        if (items.TryGetValue(id, out ItemsDataSO itemsDataSO))
        {
            return itemsDataSO;
        }
        return null;
    }
}
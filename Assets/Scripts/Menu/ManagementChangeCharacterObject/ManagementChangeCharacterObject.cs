using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagementChangeCharacterObject : MonoBehaviour
{
    public Image objectSprite;
    public TMP_Text objectAmount;
    public void SetObjectData(Sprite sprite, int amount){
        objectSprite.sprite = sprite;
        objectAmount.text = amount.ToString();
    }
}

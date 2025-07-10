using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillMenu : MonoBehaviour
{
    public Image objectSprite;
    public void SetSkillData(Sprite sprite)
    {
        objectSprite.sprite = sprite;
    }
}

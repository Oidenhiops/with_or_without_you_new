using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TMP_Text label;
    string textSelected = "";
    Color colorSelected = Color.white;
    public TypewriterByCharacter typewriterByCharacter;
    public TextAnimator_TMP textAnimator_TMP;
    public async Task SendText(string value, Color color)
    {
        textSelected = value;
        colorSelected = color;
        await Awaitable.WaitForSecondsAsync(0.1f);
        label.text = textSelected;
        label.color = colorSelected;
        typewriterByCharacter.StartShowingText();
        await Awaitable.WaitForSecondsAsync(1);
        typewriterByCharacter.StartDisappearingText();
    }
}

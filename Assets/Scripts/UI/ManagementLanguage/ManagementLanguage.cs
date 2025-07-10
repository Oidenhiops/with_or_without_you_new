using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagementLanguage : MonoBehaviour
{
    public TMP_Text dialogText;
    public int id = 0;
    [NonSerialized] public string[] dialogIds = {};
    void OnValidate()
    {
        if (dialogText == null) dialogText = GetComponent<TMP_Text>();
    }
    public GameData.TypeLanguage currentLanguage = GameData.TypeLanguage.English;
    void OnDestroy()
    {
        GameData.Instance.saveData.configurationsInfo.OnLanguageChange -= RefreshText;
    }
    void Awake()
    {
        GameData.Instance.saveData.configurationsInfo.OnLanguageChange += RefreshText;
        RefreshText();
    }
    public void RefreshText(GameData.TypeLanguage language = GameData.TypeLanguage.English)
    {
        dialogText.text = "";
        if (id != 0)
        {
            dialogText.text += GameData.Instance.GetDialog(id);
        }
        else
        {
            bool firstDialog = true;
            foreach (string dialog in dialogIds)
            {
                string text = "";
                if (int.TryParse(dialog, out int dialogId))
                {
                    text = firstDialog ? GameData.Instance.GetDialog(dialogId) : $" {GameData.Instance.GetDialog(dialogId)}";
                }
                else
                {
                    text = firstDialog ? dialog : dialog[0] == '$' ? $" {dialog.Substring(1)}" : $" {dialog}";
                }
                dialogText.text += text;
                firstDialog = false;
            }
        }
    }
}
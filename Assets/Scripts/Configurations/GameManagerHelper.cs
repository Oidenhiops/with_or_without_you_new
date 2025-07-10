using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerHelper : MonoBehaviour
{
    [NonSerialized] public bool isInitializeComponent;
    GameObject audioBoxInstance;
    [SerializeField] Animator _unloadAnimator;
    public void ChangeScene(int typeScene)
    {
        GameManager.TypeScene scene = (GameManager.TypeScene)typeScene;
        GameManager.Instance.ChangeSceneSelector(scene);
    }
    public void SaveGame()
    {
        GameData.Instance.SaveGameData();
    }
    public void PlayASound(AudioClip audioClip)
    {
        AudioManager.Instance.PlayASound(audioClip);
    }
    public void PlayASound(AudioClip audioClip, float initialRandomPitch)
    {
        AudioManager.Instance.PlayASound(audioClip, initialRandomPitch, false);
    }
    public void PlayASoundButton(AudioClip audioClip)
    {
        AudioManager.Instance.PlayASound(audioClip, 1, false);
    }
    public void PlayASoundButtonWhitInitialize(AudioClip audioClip)
    {
        if (isInitializeComponent) AudioManager.Instance.PlayASound(audioClip, 1, false);
    }
    public void PlayASoundButtonWhitInitializeAndUniqueInstance(AudioClip audioClip)
    {
        if (isInitializeComponent && audioBoxInstance == null)
        {
            AudioManager.Instance.PlayASound(audioClip, 1, false, out GameObject audioBox);
            audioBoxInstance = audioBox;
        }
    }
    public void VibrateGamePad()
    {
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.GAMEPAD)
        {
            var gamepad = Gamepad.current;
            Gamepad.current.SetMotorSpeeds(0.5f, 0.5f);
            StartCoroutine(StopVibration(gamepad));
        }
    }
    IEnumerator StopVibration(Gamepad gamepad)
    {
        if (gamepad != null)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
    public void SetAudioMixerData()
    {
        AudioManager.Instance.SetAudioMixerData();
    }
    public void UnloadScene()
    {
        string sceneForUnload = ValidateScene();
        _= UnloadSceneOptions(sceneForUnload);
    }
    public string ValidateScene()
    {
        int sceneCount = SceneManager.sceneCount;
        List<string> scenes = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            scenes.Add(SceneManager.GetSceneAt(i).name);
        }
        if (scenes.Contains("CreditsScene")) return "CreditsScene";
        return "OptionsScene";
    }
    public async Awaitable UnloadSceneOptions(string sceneForUnload)
    {
        try
        {
            _unloadAnimator.SetBool("exit", true);
            await Task.Delay(TimeSpan.FromSeconds(0.25f));
            while (_unloadAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.05));
            }
            Scene scene = SceneManager.GetSceneByName("HomeScene");
            if (scene.IsValid() && scene.isLoaded)
            {
                MenuHelper menuHelper = FindAnyObjectByType<MenuHelper>();
                if (menuHelper != null)
                {
                    menuHelper.SelectButton();
                }
            }
            if (sceneForUnload == "OptionsScene")
            {
                Time.timeScale = 1;
                GameManager.Instance.isPause = false;
            }
            _ = SceneManager.UnloadSceneAsync(sceneForUnload);
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
    }
}

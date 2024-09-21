using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Main menu UI
    public CanvasGroup mainMenuUI;  // Assign the mainMenuUI group in the Unity Inspector
    public Camera mainCamera;      // Assign the main camera in the Unity Inspector
    // Cutscene UI
    public Image cutsceneImage;
    public float fadeInDuration = 1f;
    public float displayDuration = 2f;
    public float fadeOutDuration = 1f;
    public ScenesManager.GameScene nextScene = ScenesManager.GameScene.MainGame;

    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    
    private void Start()
    {
        // Set screen resolution
        Screen.SetResolution(ScreenWidth, ScreenHeight, false);

        CenterCamera();
        // hidden cutscene upon initialization
        Color color = cutsceneImage.color;
        color.a = 0f;
        cutsceneImage.color = color;
        ShowMenuUI();
    }

    public void PLayCutscene()
    {
        HideMenuUI();
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {   
        // fade in cutscene
        yield return StartCoroutine(FadeImage(0f, 1f, fadeInDuration));
        
        yield return new WaitForSeconds(displayDuration);
        
        // fade out cutscene
        yield return StartCoroutine(FadeImage(1f, 0f, fadeOutDuration));

        ScenesManager.Instance.LoadScene(nextScene);
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = cutsceneImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            color.a = alpha;
            cutsceneImage.color = color;
            yield return null;
        }
        
        color.a = endAlpha;
        cutsceneImage.color = color;

    }
    
    private void ShowMenuUI()
    {
        if (mainMenuUI == null) return;
        mainMenuUI.alpha = 0f;
        mainMenuUI.interactable = true;
        mainMenuUI.blocksRaycasts = true;
    }
    
    private void HideMenuUI()
    {
        if (mainMenuUI == null) return;
        mainMenuUI.alpha = 1f;
        mainMenuUI.interactable = false;
        mainMenuUI.blocksRaycasts = false;
    }

    private void CenterCamera()
    {
        mainCamera.orthographicSize = ScreenHeight / 2f;
        mainCamera.transform.position = new Vector3(
            ScreenWidth / 2f, 
            ScreenHeight / 2f, 
            mainCamera.transform.position.z);
    }
}

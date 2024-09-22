using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // Main menu UI
    public CanvasGroup mainMenuUI;  // Assign mainMenuUI group in the Unity Inspector

    // Cutscene UI
    public Image cutsceneImage; 
    public float fadeInDuration = 1f; 
    public float displayDuration = 2f;
    public float fadeOutDuration = 1f;
    
    private void Start()
    {
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
    
    // Triggers cutscene animations that fades in and out at certain interval
    private IEnumerator PlayCutscene()
    {   
        // fade in cutscene
        yield return StartCoroutine(FadeImage(0f, 1f, fadeInDuration));
        // Pause cutscene for display
        yield return new WaitForSeconds(displayDuration);
        // fade out cutscene
        yield return StartCoroutine(FadeImage(1f, 0f, fadeOutDuration));

        ScenesManager.Instance.LoadNewGame(); // Transition into actual game scene
    }
    
    // helper function for the PlayCutscene
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
        mainMenuUI.alpha = 1f;
        mainMenuUI.interactable = true;
        mainMenuUI.blocksRaycasts = true;
    }
    
    private void HideMenuUI()
    {
        if (mainMenuUI == null) return;
        mainMenuUI.alpha = 0f;
        mainMenuUI.interactable = false;
        mainMenuUI.blocksRaycasts = false;
    }

}

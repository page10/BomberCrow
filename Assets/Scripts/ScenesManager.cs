using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Responsible for managing scenes
public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    public Animator transition; // Assign transition in Unity Inspector
    public float transitionTime = 1f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        } 
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public enum GameScene
    {
        MainMenu,
        MainGame,
        GameOver,
        Winning,
    }

    private static void LoadScene(GameScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNewGame()
    {
        StartCoroutine(LoadAsyncScene(GameScene.MainGame));
        
    }
    
    // trigger game over scene
    public void LoadGameOver()
    {
        StartCoroutine(LoadAsyncScene(GameScene.GameOver));
    }
    
    // triggered winning scene
    public void LoadWinning()
    {
        StartCoroutine(LoadAsyncScene(GameScene.Winning));
    }

    public void LoadMainMenu()
    {
        // SceneManager.LoadScene(GameScene.MainMenu.ToString());
        StartCoroutine(LoadAsyncScene(GameScene.MainMenu));
    }

    private IEnumerator LoadAsyncScene(GameScene scene)
    {
        transition.SetTrigger("Start");
        
        yield return new WaitForSeconds(transitionTime);
        
        LoadScene(scene);
    }
}

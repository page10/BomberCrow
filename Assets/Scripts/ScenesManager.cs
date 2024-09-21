using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Responsible for managing scenes
public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public enum GameScene
    {
        MainMenu,
        GameOver,
        MainGame
    }

    public void LoadScene(GameScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNewGame()
    {
        SceneManager.LoadScene(GameScene.MainMenu.ToString());
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene(GameScene.GameOver.ToString());
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene(GameScene.MainGame.ToString());
    }
    
    
}

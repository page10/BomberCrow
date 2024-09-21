
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIMainMenuController : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button quitGame;
    public MainMenuManager mainMenuManager;

    private void Start()
    {
        // Set up buttons
        if (newGame != null)
        {
            newGame.onClick.AddListener(OnStartButtonClicked);
        }

        if (quitGame != null)
        {
            quitGame.onClick.AddListener(OnQuitButtonClicked);
        }
    }
    private void OnStartButtonClicked()
    {
        mainMenuManager.PLayCutscene();
    }

    private static void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMainMenuController : MonoBehaviour
    {
        [SerializeField] private Button newGame;
        [SerializeField] private Button quitGame;
        public MainMenuManager mainMenuManager;

        private void Start()
        {
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
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #else
    Application.Quit();
    #endif
}
    }
}
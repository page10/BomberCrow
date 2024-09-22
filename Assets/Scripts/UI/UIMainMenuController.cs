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
            // commented in once built. Responsible for quitting the game
            // Application.Quit();
        
            // Reserved for testing in play mode
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
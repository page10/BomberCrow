using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIGameOverController : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnMenuButton;
        private ScoreManager _scoreManager;
        public TextMeshProUGUI scoreText;


        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            }

            if (returnMenuButton != null)
            {
                returnMenuButton.onClick.AddListener(OnReturnMenuButtonClicked);
            }
            
            _scoreManager = FindObjectOfType<ScoreManager>();
            UpdateScoreText();
        }
        
        private void UpdateScoreText()
        {
            scoreText.text = "Snow pile Blasted: " + _scoreManager.SnowPileBlasted + "\n" +
                             "Enemy Slaughtered: " + _scoreManager.EnemySlaughtered + "\n" +
                             "Fruit Picked: " + _scoreManager.FruitPicked;
        }
    
        private void OnRestartButtonClicked()
        {
            ScenesManager.Instance.LoadNewGame();
        }

        private void OnReturnMenuButtonClicked()
        {
            ScenesManager.Instance.LoadMainMenu();
        }
    }
}

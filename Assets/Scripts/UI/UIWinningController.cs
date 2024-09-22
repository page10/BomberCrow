using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIWinningController : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button returnMenuButton;
        private ScoreManager _scoreManager;
        public TextMeshProUGUI scoreText;

        private void Start()
        {
            if (newGameButton != null)
            {
                newGameButton.onClick.AddListener(OnNewButtonClicked);
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
    
        private void OnNewButtonClicked()
        {
            ScenesManager.Instance.LoadNewGame();
        }

        private void OnReturnMenuButtonClicked()
        {
            ScenesManager.Instance.LoadMainMenu();
        }
    }
}

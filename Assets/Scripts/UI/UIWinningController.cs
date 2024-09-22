using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIWinningController : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button returnMenuButton;
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
            
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            scoreText.text = "Snow pile Blasted: " + ScoreManager.Instance.SnowPileBlasted + "\n" +
                             "Enemy Slaughtered: " + ScoreManager.Instance.EnemySlaughtered + "\n" +
                             "Fruit Picked: " + ScoreManager.Instance.FruitPicked;
        }
        
    
        private void OnNewButtonClicked()
        {
            ScenesManager.Instance.LoadNewGame();
            ScoreManager.Instance.ResetScore();
        }

        private void OnReturnMenuButtonClicked()
        {
            ScenesManager.Instance.LoadMainMenu();
            ScoreManager.Instance.ResetScore();
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIGameOverController : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnMenuButton;
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
            
            if (scoreText == null)
            {
                Debug.Log(scoreText);
                Debug.Log(ScoreManager.Instance.SnowPileBlasted);
            }
            UpdateScoreText();
        }
        
        private void UpdateScoreText()
        {
            Debug.Log(scoreText.text);
            scoreText.text = "Snow pile Blasted: " + ScoreManager.Instance.SnowPileBlasted + "\n" +
                             "Enemy Slaughtered: " + ScoreManager.Instance.EnemySlaughtered + "\n" +
                             "Fruit Picked: " + ScoreManager.Instance.FruitPicked;
        }
    
        private void OnRestartButtonClicked()
        {
            ScenesManager.Instance.LoadNewGame();
            ScoreManager.Instance.ResetScore();
        }

        private void OnReturnMenuButtonClicked()
        {
            ScenesManager.Instance.LoadMainMenu();
        }
    }
}

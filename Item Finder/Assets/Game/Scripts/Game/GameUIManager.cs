using UnityEngine;
using TMPro;

namespace NillaStudios
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("Main UI Panels")]
        public GameObject getReadyPanel;
        public GameObject gamePanel;
        public GameObject gameOverPanel;
        public GameObject levelPassedPanel;
        public GameObject classUnlockPanel;
        public GameObject pauseMenuPanel;
        public GameObject noLivesMenuPanel;
        public GameObject noCoinsMenuPanel;

        [Header("Game Panel")]
        public TextMeshProUGUI coinText;
        public TextMeshProUGUI timerText;

        [Header("Class Unlock Panel")]
        public TextMeshProUGUI classUnlockText;

        private GameController gameController;

        private void Awake()
        {
            gameController = FindObjectOfType<GameController>();
        }

        public void StartGame()
        {
            getReadyPanel.SetActive(false);
            gamePanel.SetActive(true);
        }

        public void OpenPauseMenu()
        {
            // Make everything pause here
            
            pauseMenuPanel.SetActive(true);
        }

        public void ClosePauseMenu()
        {
            pauseMenuPanel.SetActive(false);
        }

        public void SetTimerText(string timer)
        {
            timer += "s";
            timerText.text = timer;
        }

        public void SetCoinText(string coins)
        {
            coinText.text = coins;
        }

        public void GameOver()
        {
            gamePanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        public void LevelPassed()
        {
            gamePanel.SetActive(false);
            levelPassedPanel.SetActive(true);
        }

        public void LevelPassedContinue()
        {
            levelPassedPanel.SetActive(false);
            classUnlockPanel.SetActive(true);

            classUnlockText.text = gameController.GetNewClassLetterString();
        }

        public void ClassUnlockContinue()
        {
            // We have passed all steps, goto next level
            gameController.NextLevel();
        }

        public void PauseGame()
        {
            pauseMenuPanel.SetActive(true);

            gameController.PauseGame();
        }

        public void PauseMenuResume()
        {
            pauseMenuPanel.SetActive(false);

            gameController.ResumeGame();
        }

        public void PauseMenuRestart()
        {
            gameController.ResumeGame();
            gameController.RestartLevel();
        }

        public void PauseMenuHome()
        {
            gameController.ResumeGame();
            gameController.GotoHome();
        }

        public void GameOverRestart()
        {
            gameController.ResumeGame();
            gameController.RestartLevel();
        }

        public void GameOverHome()
        {
            gameController.ResumeGame();
            gameController.GotoHome();
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace NillaStudios
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI livesText;
        public TextMeshProUGUI livesRestoreTimerText;
        public TextMeshProUGUI coinsText;
        public TextMeshProUGUI levelText;

        private void Start()
        {
            levelText.text = PlayerPrefs.GetInt("LevelNumber").ToString();       
            coinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString(); 
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Game");
        }

        private void Update()
        {
            livesText.text = LifeSystemController.instance.livesLeft.ToString();
            livesRestoreTimerText.text = LifeSystemController.instance.timeLeftfor5Lives.ToString("0");
        }
    }
}

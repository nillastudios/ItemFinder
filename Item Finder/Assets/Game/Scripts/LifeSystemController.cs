using System;
using UnityEngine;

namespace NillaStudios
{
    public class LifeSystemController : MonoBehaviour
    {
        public int maxLives = 5;
        public int livesLeft;
        public float timeFor1Life = 300f;

        public float timeLeftfor5Lives;
        public double timePassed;

        public DateTime lastGameCloseTime;

        public static LifeSystemController instance;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // get last time when the game was closed
            lastGameCloseTime = Convert.ToDateTime(PlayerPrefs.GetString("LastGameCloseTime"));
            timePassed = DateTime.Now.Subtract(lastGameCloseTime).TotalSeconds;
            timeLeftfor5Lives = PlayerPrefs.GetFloat("TimeLeftFor5Lives", 0f);

            // Now calculate amount of lives need to added and remaining time
            if(timeLeftfor5Lives - timePassed <= 0f)
            {
                livesLeft = maxLives;
                timeLeftfor5Lives = 0f;
            }          
            else
            {
                timeLeftfor5Lives -= (float)timePassed;
                livesLeft = Convert.ToInt32((timeFor1Life * maxLives - timeLeftfor5Lives) % (timeFor1Life));
            }
        }

        private void Update()
        {
            if(timeLeftfor5Lives > 0f)
            {
                timeLeftfor5Lives -= Time.deltaTime;

                // calculate no of lives left
                livesLeft = (int)((timeFor1Life * maxLives - timeLeftfor5Lives) / (timeFor1Life));

                // Time for 5 lives is complete
                if(timeLeftfor5Lives <= 0f)
                {
                    livesLeft = maxLives;
                    timeLeftfor5Lives = 0f;
                }
            }
        }

        [ContextMenu("Lose Life")]
        public bool LoseLife()
        {
            // if there is no health left just return
            if(livesLeft <= 0)
            {
                return false;
            }

            // Set timer to a new value
            timeLeftfor5Lives += timeFor1Life;

            return true;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
            {
                PlayerPrefs.SetFloat("TimeLeftFor5Lives", timeLeftfor5Lives);
                PlayerPrefs.SetString("LastGameCloseTime", DateTime.Now.ToString());
                PlayerPrefs.Save();
            }
            else
            {
                Start();
            }
        }

        private void OnApplicationQuit()
        {
            OnApplicationPause(true);
        }
    }
}

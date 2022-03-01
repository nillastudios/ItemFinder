using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace NillaStudios
{
    public class GameController : MonoBehaviour
    {
        public PickableObject[] objects;
        public int currentClass;
        public string objToFind = string.Empty;
        public Transform spawnPoint;

        public List<SpawnableObjectsClass> spawnableObjectsClasses = new List<SpawnableObjectsClass>();
        public List<SpawnableObjectsClass> unlockedSpawnableClasses = new List<SpawnableObjectsClass>();
        public List<PickableObject> requiredObjects = new List<PickableObject>();
        public List<PickableObject> nonClassObjects = new List<PickableObject>();
        private List<PickableObject> spawnList = new List<PickableObject>();
        public int levelNumber;

        public float timeForEachObject = 5f;
        private float timer;
        private bool runTimer;
        private bool levelPassed;

        private float lastCoinCollectTimeStamp;
        public float timeForMultiplierToGoTo1 = 3f;
        private int coinsEarnedThisLevel;

        private GameUIManager gameUIManager;
        private DragObject dragObject;
        private ObjectReceiver objectReceiver;

        private void Awake()
        {
            gameUIManager = FindObjectOfType<GameUIManager>();
            objectReceiver = FindObjectOfType<ObjectReceiver>();
            dragObject = FindObjectOfType<DragObject>();
        }

        private void Start()
        {
            // Reset time scale in case it was different
            Time.timeScale = 1f;

            if(PlayerPrefs.GetInt("FirstGame") == 0)
            {
                PlayerPrefs.SetInt("YellowStatus", 1);
            }

            // Sort pickable objects so we can make level design easy
            SortPickable();

            // Generate level using level number as a seed
            levelNumber = PlayerPrefs.GetInt("LevelNumber");
            GenerateLevel(levelNumber);

            // Start the game, we are done
            // Game will start after every object is spawned
        }

        public void SortPickable()
        {
            // Sort everything first
            for(int i = 0; i < objects.Length; i++)
            {
                // Try getting the objects class
                SpawnableObjectsClass objectsClass = spawnableObjectsClasses.Find(x => x.objectClass == objects[i].objectClass); 
                
                // If object class already exists
                if(objectsClass != null)
                {
                    // Add current object to the class
                    objectsClass.spawnableObjects.Add(objects[i]);
                } 
                // If it doesn't exist then create a class
                else
                {
                    // Create new class and add current object to it
                    SpawnableObjectsClass newSpawnableObjClass = new SpawnableObjectsClass { name = objects[i].objectClass.ToString(), objectClass = objects[i].objectClass };
                    newSpawnableObjClass.spawnableObjects.Add(objects[i]);

                    // Add new class to the list                    
                    spawnableObjectsClasses.Add(newSpawnableObjClass);
                }         
            }

            // Determine whether the current class is unlocked or not
            for(int i = 0; i < spawnableObjectsClasses.Count; i++)
            {
                // 0 = locked, 1 = unlocked
                int classStatus = PlayerPrefs.GetInt(spawnableObjectsClasses[i].objectClass.ToString() + "Status", 0);
                if(classStatus == 1)
                {
                    spawnableObjectsClasses[i].unlocked = true;
                    spawnableObjectsClasses[i].unlockPercentage = 1f;

                    // Add this class to unlocked classes to generate level
                    unlockedSpawnableClasses.Add(spawnableObjectsClasses[i]);
                }
            }
        }

        private void GenerateLevel(int seed)
        {
            // Get a class to find 
            currentClass = RandomNumberDeterministic(seed, 0, unlockedSpawnableClasses.Count);

            // Get number of objects to spawn
            int noOfObjectsToFind = seed > 20 ? RandomNumberDeterministic(seed, 8, 26) : RandomNumberDeterministic(seed, 3, 13);
            int noOfnonClassObjects = seed > 20 ? RandomNumberDeterministic(seed, 10, 26) : RandomNumberDeterministic(seed, 20, 51);

            // This is a bonus level, do crazy shit
            if(isBonusLevel())
            {
                Debug.Log("hello mf");
                PickableObject obj = unlockedSpawnableClasses[currentClass].spawnableObjects[RandomNumberDeterministic(seed, 0, unlockedSpawnableClasses[currentClass].spawnableObjects.Count)];
                objToFind = obj.objectName;

                // Adjust objects to find
                noOfObjectsToFind = 0;
                noOfnonClassObjects = RandomNumberDeterministic(seed, 30, 51);

                // Add this to spawn list
                spawnList.Add(obj);
            }

            // Make all objects other than current class as non class obj
            for(int i = 0; i < spawnableObjectsClasses.Count; i++)
            {
                if(unlockedSpawnableClasses[currentClass].objectClass != spawnableObjectsClasses[i].objectClass)
                {
                    for(int j = 0; j < spawnableObjectsClasses[i].spawnableObjects.Count; j++)
                    {
                        nonClassObjects.Add(spawnableObjectsClasses[i].spawnableObjects[j]);
                    }
                }
            }

            // Spawn random objects from selected class
            for(int i = 0; i < noOfObjectsToFind; i++)
            {
                // Get the item to spawn
                PickableObject itemPref = unlockedSpawnableClasses[currentClass]
                .spawnableObjects[RandomNumberDeterministic((currentClass + i + noOfnonClassObjects) * (seed + i), 0, unlockedSpawnableClasses[currentClass].spawnableObjects.Count)]; 

                // Add it to spawn list
                spawnList.Add(itemPref);           
            }

            // Spawn random objects from pickable objects
            for(int i = 0; i < noOfnonClassObjects; i++)
            {
                // Get the item to spawn
                PickableObject itemPref = nonClassObjects[RandomNumberDeterministic(seed + noOfObjectsToFind + i, 0, nonClassObjects.Count)];

                // Add it to spawn list
                spawnList.Add(itemPref);           
            }

            // Randomize list so it doesn't spawn in order
            spawnList.Shuffle();

            // Setup object receiever
            objectReceiver.Initialize(spawnableObjectsClasses[currentClass].objectClass, timeForMultiplierToGoTo1, this, objToFind);

            // Spawn them in order
            StartCoroutine(StartGame(0.05f));
        }

        private IEnumerator StartGame(float timeStep)
        {
            for(int i = 0; i < spawnList.Count; i++)
            {
                // Spawn required objects in a cool way
                GameObject itemIns = Instantiate(spawnList[i].gameObject, spawnPoint.position, Quaternion.identity);

                // If we need to find current object add it to seperate list
                if(spawnableObjectsClasses[currentClass].spawnableObjects.Contains(spawnList[i]))
                {
                    requiredObjects.Add(itemIns.GetComponentInChildren<PickableObject>());
                }

                Vector3 sideForce = new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, 0f);
                itemIns.GetComponentInChildren<Rigidbody>().AddForce((spawnPoint.forward + sideForce) * 50f, ForceMode.VelocityChange);
                
                yield return new WaitForSeconds(timeStep);
            }

            // Start Game Here
            StartGame();

            yield break;
        }

        private void StartGame()
        {
            // Turn on correct UI
            gameUIManager.StartGame();

            // Start timer
            timer = requiredObjects.Count * timeForEachObject;

            // if bonus level use different scheme, flat 10s time
            if(isBonusLevel())
            {
                timer = 10f;
            }

            runTimer = true;
        }

        private void Update()
        {
            if(runTimer)
            {
                // Reduce timer if game has started
                timer -= Time.deltaTime;
                gameUIManager.SetTimerText(Mathf.RoundToInt(timer).ToString());

                if(timer <= 0)
                {
                    // if(requiredObjects.Count <= 0)
                    // {
                    //     // level passed game over
                    //     LevelPassed();
                        
                    // }
                    // else
                    // {
                        // Level failed game over
                        GameOver();
                    // }
                }
            }
        }

        public void ObjectReceived(PickableObject receivedObj, int coinsEarned)
        {
            if(requiredObjects.Contains(receivedObj))
            {
                requiredObjects.Remove(receivedObj);
                Destroy(receivedObj.gameObject);

                objectReceiver.FillCoinMeter();

                // Add coins 
                coinsEarnedThisLevel += coinsEarned;
                gameUIManager.SetCoinText(coinsEarnedThisLevel.ToString());
            }
            else
            {
                dragObject.ClearSelectedRigidbody();
                receivedObj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 1f, 1f) * 15f;
            }

            if(requiredObjects.Count <= 0)
            {
                // Level passed
                LevelPassed();
            }
        }

        public void GameOver()
        {
            // how game over if level is passed
            if(levelPassed)
            {
                return;
            }

            // Lose a life
            // LifeSystemController.instance.LoseLife();

            // Change UI
            gameUIManager.GameOver();

            // Save coins earned
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coinsEarnedThisLevel);
        }

        public void LevelPassed()
        {
            // Change UI
            gameUIManager.LevelPassed();

            // Save coins earned
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coinsEarnedThisLevel);

            // set level passed so it won't get to game over
            levelPassed = true;
            runTimer = false;
        }

        public string GetNewClassLetterString()
        {
            string returnString = string.Empty;

            var objClasses = Enum.GetValues(typeof(ObjectClass));
            foreach(var objClass in objClasses)
            {
                // if already unlocked then just skip
                if(PlayerPrefs.GetInt(objClass.ToString() + "Status") == 1)
                {
                    continue;
                }

                // Check for all letters for unlockable class
                string className = objClass.ToString();
                for(int i = 0; i < className.Length; i++)
                {
                    // Check if that letter was unlocked
                    if(PlayerPrefs.GetInt(className + "Status" + className[i] + i.ToString(), 0) == 0)
                    {
                        // Yeah we found the letter we need, just unlock it
                        PlayerPrefs.SetInt(className + "Status" + className[i] + i.ToString(), 1);
                        returnString += className[i];

                        // Just fill the empty gaps with ? as a sign of suspense
                        for(int j = 0; j < className.Length - i - 1; j++)
                        {
                            returnString += "?";
                        }

                        // If it is the final letter, unlock the new class
                        if(i == className.Length - 1)
                        {
                            PlayerPrefs.SetInt(className + "Status", 1);                            
                        }

                        // That's it
                        return returnString;
                    }
                    else
                    {
                        // If already unlocked
                        returnString += className[i];
                    }
                }
            }

            return returnString;
        }

        public bool isBonusLevel()
        {
            var objClasses = Enum.GetValues(typeof(ObjectClass));
            foreach(var objClass in objClasses)
            {
                // if already unlocked then just skip
                if(PlayerPrefs.GetInt(objClass.ToString() + "Status") == 1)
                {
                    continue;
                }

                // Check for all letters for unlockable class
                string className = objClass.ToString();
                for(int i = 0; i < className.Length; i++)
                {
                    // Check if that letter was unlocked
                    if(PlayerPrefs.GetInt(className + "Status" + className[i] + i.ToString(), 0) == 0)
                    {
                        if(i == className.Length - 1)
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public void NextLevel()
        {
            // Increment level number
            PlayerPrefs.SetInt("LevelNumber", PlayerPrefs.GetInt("LevelNumber") + 1);

            // Goto next level
            SceneManager.LoadScene("Game");
        }

        public void RestartLevel()
        {
            // Just restart the scene
            SceneManager.LoadScene("Game");
        }

        public void PauseGame()
        {
            // make the time scale as 0
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            // make the time scale as 1
            Time.timeScale = 1f;
        }

        public void GotoHome()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public int RandomNumberDeterministic(int seed, int min, int max)
        {
            System.Random random = new System.Random(seed);
            return random.Next(min, max);
        }

        
    }

    [System.Serializable]
    public class SpawnableObjectsClass
    {
        public string name;
        public ObjectClass objectClass;
        public List<PickableObject> spawnableObjects = new List<PickableObject>();
        public bool unlocked;
        public float unlockPercentage;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight
{
    public class GameManager : MonoBehaviour
    {
        public string transitionedFromScene;
        public Vector2 platformingRespawnPoint;
        public GameObject defaultRespawnPoint;
        public Vector2 respawnPoint;
        [SerializeField] Bench bench;
        public GameObject shade;

        [SerializeField] private FadeUI pauseMenu;
        [SerializeField] private float fadeTime;
        public bool gameIsPaused;

        // Singleton instance
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            // Check if instance already exists
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Destroy duplicate instance
            }
            else
            {
                Instance = this;
                SaveData.Instance.Initialize();
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SaveData.Instance.SavePlayer();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
            {
                pauseMenu.FadeUIIn(fadeTime);
                Time.timeScale = 0f; // Pause the game
                gameIsPaused = true;
            }
        }

        public void UnPauseGame()
        {
            Time.timeScale = 1f; // Resume the game
            gameIsPaused = false;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bench = FindObjectOfType<Bench>();
            defaultRespawnPoint = GameObject.Find("DefaultRespawnPoint");
            platformingRespawnPoint = defaultRespawnPoint.transform.position;

            SaveScene();
            if (PlayerController.Instance != null)
            {
                if (PlayerController.Instance.halfMana)
                {
                    SaveData.Instance.LoadShade();
                    if (SaveData.Instance.sceneWithShade == SceneManager.GetActiveScene().name || SaveData.Instance.sceneWithShade == "")
                    {
                        Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
                    }
                }
            }
        }

        public void RespawnPlayer()
        {
            SaveData.Instance.LoadBench();

            // Load the bench scene if it exists
            if (SaveData.Instance.benchSceneName != null)
            {
                SceneManager.LoadScene(SaveData.Instance.benchSceneName);
            }

            // Set the respawn point to the bench position if it exists
            // otherwise use the last platforming respawn point
            if (SaveData.Instance.benchPos != null)
            {
                respawnPoint = SaveData.Instance.benchPos;
            }
            else
            {
                if (platformingRespawnPoint != null) {
                    respawnPoint = platformingRespawnPoint;
                } else {
                    respawnPoint = new Vector2(defaultRespawnPoint.transform.position.x, defaultRespawnPoint.transform.position.y);
                }
            }

            PlayerController.Instance.transform.position = respawnPoint;
            StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
            PlayerController.Instance.Respawned();
        }

        public void SaveScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.discoveredSceneNames.Add(currentSceneName);
            SaveData.Instance.SaveDiscoveredMaps();
        }
    }
}

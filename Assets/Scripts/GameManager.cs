using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

namespace Metroknight
{
    public class GameManager : MonoBehaviour
    {
        public string transitionedFromScene;
        public Vector2 platformingRespawnPoint;
        public Vector2 respawnPoint;
        [SerializeField] Bench bench;

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
            }
            DontDestroyOnLoad(gameObject); // Persist across scenes
            bench = FindObjectOfType<Bench>();
        }

        public void RespawnPlayer()
        {
            if (bench != null && bench.interacted)
            {
                Debug.Log("Player set bench position");
                respawnPoint = bench.transform.position;
            }
            else
            {
                Debug.Log("Player reset respawn to the zone point");
                respawnPoint = platformingRespawnPoint;
            }
            PlayerController.Instance.transform.position = respawnPoint;
            StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
            PlayerController.Instance.Respawned();
        }
    }
}

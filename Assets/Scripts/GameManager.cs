using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

namespace Metroknight
{
public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

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
    }
}
}
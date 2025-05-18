using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight {
public class UIManager : MonoBehaviour
{

    public SceneFader sceneFader;
    // Singleton instance
    public static UIManager Instance;

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

    private void Start()
    {
      sceneFader = GetComponentInChildren<SceneFader>();
    }
  }
}

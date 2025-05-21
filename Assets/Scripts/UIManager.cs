using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight {
  [HideInInspector]
  public class UIManager : MonoBehaviour
  {

    public SceneFader sceneFader;
    [SerializeField] GameObject deathScreen;
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
      DontDestroyOnLoad(gameObject);

      sceneFader = GetComponentInChildren<SceneFader>();// Persist across scenes
    }

    public IEnumerator ActivateDeathScreen()
    {
      yield return new WaitForSeconds(0.8f);
      StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));
      yield return new WaitForSeconds(0.8f);
      deathScreen.SetActive(true);
    }

    public IEnumerator DeactivateDeathScreen()
    {
      yield return new WaitForSeconds(0.5f);
      deathScreen.SetActive(false);
      StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }
  }
}

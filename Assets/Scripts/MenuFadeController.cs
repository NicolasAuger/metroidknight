using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight
{
    public class MenuFadeController : MonoBehaviour
    {
        private FadeUI fadeUI;
        [SerializeField] private float fadeTime;

        void Start()
        {
            fadeUI = GetComponent<FadeUI>();
            fadeUI.FadeUIOut(fadeTime);
        }

        public void CallFadeAndStartGame(string _sceneToLoad)
        {
            StartCoroutine(FadeAndStartGame(_sceneToLoad));
        }

        IEnumerator FadeAndStartGame(string _sceneToLoad)
        {
            fadeUI.FadeUIIn(fadeTime);
            // Fade waiting
            yield return new WaitForSeconds(fadeTime);
            // Black screen waiting
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(_sceneToLoad);
        }
    }
}

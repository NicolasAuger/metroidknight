using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class FadeUI : MonoBehaviour
    {
        CanvasGroup canvasGroup;


        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeUIIn(float _seconds)
        {
            StartCoroutine(FadeIn(_seconds));
        }

        public void FadeUIOut(float _seconds)
        {
            StartCoroutine(FadeOut(_seconds));
        }

        IEnumerator FadeOut(float _seconds)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 1f;

            while (canvasGroup.alpha > 0f)
            {
                // Since game will probably be paused when this is called, we need to use unscaledDeltaTime
                canvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
                yield return null;
            }
            yield return null;
        }

        IEnumerator FadeIn(float _seconds)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 0f;

            while (canvasGroup.alpha < 1f)
            {
                // Since game will probably be paused when this is called, we need to use unscaledDeltaTime
                canvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
                yield return null;
            }
            yield return null;
        }
    }
}

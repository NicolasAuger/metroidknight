using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Metroknight
{
    public class OrbShard : MonoBehaviour
    {
        public Image fill;

        public float targetFillAmount;
        public float lerpDuration = 1.5f;
        public float initialFillAmount;


        public IEnumerator LerpFill()
        {
            float elapsedTime = 0f;

            while (elapsedTime < lerpDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / lerpDuration);
                float lerpedFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, t);
                fill.fillAmount = lerpedFillAmount;
                yield return null;
            }

            // Ensure final value is set correctly
            fill.fillAmount = targetFillAmount;

            if (fill.fillAmount == 1)
            {
                PlayerController.Instance.manaOrbs++;
                PlayerController.Instance.orbShards = 0;
            }
        }
    }
}

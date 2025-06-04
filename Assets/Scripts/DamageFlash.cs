using System.Collections;
using UnityEngine;

namespace Metroknight
{
    public class DamageFlash : MonoBehaviour
    {
        [ColorUsage(true, true)]
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashTime = .25f;
        [SerializeField] private AnimationCurve flashSpeedCurve;

        private SpriteRenderer sr;
        private Material material;

        private Coroutine damageFlashCoroutine;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            material = sr.material;
        }

        public void CallDamageFlash()
        {
            // Start the coroutine to flash the damage effect
            damageFlashCoroutine = StartCoroutine(DamageFlasher());
        }

        private IEnumerator DamageFlasher()
        {
            // Set the flash color
            SetFlashColor();

            // Lerp the color to the flash color
            float elapsedTime = 0f;
            while (elapsedTime < flashTime)
            {
                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Lerp the flash amount
                float currentFlashAmount = Mathf.Lerp(1f, flashSpeedCurve.Evaluate(elapsedTime), elapsedTime / flashTime);
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }

        }

        private void SetFlashColor()
        {
            material.SetColor("_FlashColor", flashColor);
        }
        
        private void SetFlashAmount(float _amount)
        {
            material.SetFloat("_FlashAmount", _amount);
        }
    }
}
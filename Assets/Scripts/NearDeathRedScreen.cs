using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace Metroknight
{
    public class NearDeathRedScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScriptableRendererFeature nearDeathScreen;
        [SerializeField] private Material material;
        [SerializeField] private float fadeOutTime = 1f;

        private int voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");
        private int vignetteIntensity = Shader.PropertyToID("_VignetteIntensity");

        private const float VORONOI_INTENSITY_START_AMOUNT = 1f;
        private const float VIGNETTE_INTENSITY_START_AMOUNT = 1.5f;

        private bool nearDeathScreenEnabled = false;
        private bool isFadingOut = false;
        private bool isFadingIn = false;
        private bool isDraining = false;
        private float bloodTimer = 2f;
        private float currentBloodDrainTime = 0f;
        private Coroutine fadeCoroutine;
        private Coroutine bloodDrainCoroutine;

        void Start()
        {
            nearDeathScreen.SetActive(false);
        }

        void Update()
        {
            bool shouldBeActive = PlayerController.Instance.Health == 1 && PlayerController.Instance.pState.alive;

            if (shouldBeActive && !nearDeathScreenEnabled && !isFadingOut && !isFadingIn)
            {
                EnableNearDeathScreen();
            }
            else if (!shouldBeActive && nearDeathScreenEnabled && !isFadingOut)
            {
                DisableNearDeathScreen();
            }
            else if (shouldBeActive && nearDeathScreenEnabled && !isFadingIn && !isFadingOut && !isDraining)
            {
                SimulateBloodDrain();
            }
        }

        private void EnableNearDeathScreen()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            isFadingIn = true;
            material.SetFloat(voronoiIntensity, 0f);
            material.SetFloat(vignetteIntensity, 0f);
            fadeCoroutine = StartCoroutine(FadeInCoroutine());
        }

        private void DisableNearDeathScreen()
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            if (bloodDrainCoroutine != null) StopCoroutine(bloodDrainCoroutine);
            currentBloodDrainTime = bloodTimer;
            isDraining = false;
            isFadingOut = true;
            fadeCoroutine = StartCoroutine(FadeOutCoroutine());
        }

        private IEnumerator FadeInCoroutine()
        {
            float _elapsedTime = 0f;
            nearDeathScreenEnabled = true;
            nearDeathScreen.SetActive(true);

            while (_elapsedTime < .5f)
            {
                _elapsedTime += Time.deltaTime;
                float t = _elapsedTime / .5f;

                float lerpVoronoi = Mathf.Lerp(0f, VORONOI_INTENSITY_START_AMOUNT, t);
                float lerpVignette = Mathf.Lerp(0f, VIGNETTE_INTENSITY_START_AMOUNT, t);

                material.SetFloat(voronoiIntensity, lerpVoronoi);
                material.SetFloat(vignetteIntensity, lerpVignette);

                yield return null;
            }

            // Ensure final values are set to start amounts
            material.SetFloat(voronoiIntensity, VORONOI_INTENSITY_START_AMOUNT);
            material.SetFloat(vignetteIntensity, VIGNETTE_INTENSITY_START_AMOUNT);
            currentBloodDrainTime = bloodTimer;
            isFadingIn = false;
        }

        private IEnumerator FadeOutCoroutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeOutTime;

                float lerpVoronoi = Mathf.Lerp(VORONOI_INTENSITY_START_AMOUNT, 0f, t);
                float lerpVignette = Mathf.Lerp(VIGNETTE_INTENSITY_START_AMOUNT, 0f, t);

                material.SetFloat(voronoiIntensity, lerpVoronoi);
                material.SetFloat(vignetteIntensity, lerpVignette);

                yield return null;
            }

            // Ensure final values are 0
            material.SetFloat(voronoiIntensity, 0f);
            material.SetFloat(vignetteIntensity, 0f);
            nearDeathScreen.SetActive(false);
            isFadingOut = false;
            nearDeathScreenEnabled = false;
        }

        private void SimulateBloodDrain()
        {
            if (currentBloodDrainTime <= 0f)
            {
                if (bloodDrainCoroutine != null) StopCoroutine(bloodDrainCoroutine);
                bloodDrainCoroutine = StartCoroutine(BloodDrainCoroutine());
            }
            else
            {
                currentBloodDrainTime -= Time.deltaTime;
            }
        }

        private IEnumerator BloodDrainCoroutine()
        {
            Debug.Log("Simulating blood drain effect");
            isDraining = true;
            float _elapsedTime = 0f;
            float _randomDrainPower = Random.Range(1.5f, 2.5f);

            while (_elapsedTime < .25f)
            {
                _elapsedTime += Time.deltaTime;
                float t = _elapsedTime / .25f;

                float lerpVoronoi = Mathf.Lerp(VORONOI_INTENSITY_START_AMOUNT, _randomDrainPower, t);
                float lerpVignette = Mathf.Lerp(VIGNETTE_INTENSITY_START_AMOUNT, _randomDrainPower, t);

                material.SetFloat(voronoiIntensity, lerpVoronoi);
                material.SetFloat(vignetteIntensity, lerpVignette);

                yield return null;
            }

            material.SetFloat(voronoiIntensity, _randomDrainPower);
            material.SetFloat(vignetteIntensity, _randomDrainPower);
            _elapsedTime = 0f;

            while (_elapsedTime < .25f)
            {
                _elapsedTime += Time.deltaTime;
                float t = _elapsedTime / .25f;

                float lerpVoronoi = Mathf.Lerp(_randomDrainPower, VORONOI_INTENSITY_START_AMOUNT, t);
                float lerpVignette = Mathf.Lerp(_randomDrainPower, VIGNETTE_INTENSITY_START_AMOUNT, t);

                material.SetFloat(voronoiIntensity, lerpVoronoi);
                material.SetFloat(vignetteIntensity, lerpVignette);

                yield return null;
            }

            material.SetFloat(voronoiIntensity, VORONOI_INTENSITY_START_AMOUNT);
            material.SetFloat(vignetteIntensity, VIGNETTE_INTENSITY_START_AMOUNT);
            currentBloodDrainTime = bloodTimer;
            isDraining = false;
        }
    }
}
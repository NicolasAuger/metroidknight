using UnityEngine;
using UnityEngine.Audio;

namespace Metroknight
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] AudioMixer audioMixer;

        public void SetVolume(float _volume)
        {
            audioMixer.SetFloat("Volume", _volume);
        }

        public void SetQuality(int _qualityIndex)
        {
            QualitySettings.SetQualityLevel(_qualityIndex);
        }

        public void SetFullscreen(bool _isFullscreen)
        {
            Screen.fullScreen = _isFullscreen;
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
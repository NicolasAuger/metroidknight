using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class UIAudio : MonoBehaviour
    {
        [SerializeField] AudioClip hover, click, gameStart;
        AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SoundOnHover()
        {
            audioSource.PlayOneShot(hover);
        }

        public void SoundOnClick()
        {
            audioSource.PlayOneShot(click);
        }

        public void SoundOnGameStartClick()
        {
            audioSource.PlayOneShot(gameStart);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Metroknight
{
    public class Lantern : MonoBehaviour
    {

        private Animator animator;
        private Light2D lightSource;
        private AudioSource audioSource;
        [SerializeField] private AudioClip lanternLightedSound;
        private bool isLighted = false;

        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponent<Animator>();
            lightSource = GetComponent<Light2D>();
            audioSource = GetComponent<AudioSource>();
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !isLighted)
            {
                animator.enabled = true;
                animator.Play("Lantern_Idle");
                audioSource.PlayOneShot(lanternLightedSound);
                isLighted = true;
                lightSource.enabled = true;
            }
        }
  }
}
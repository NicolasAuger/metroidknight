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

        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponent<Animator>();
            lightSource = GetComponent<Light2D>();
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                animator.enabled = true;
                animator.Play("Lantern_Idle");
                lightSource.enabled = true;
            }
        }
  }
}
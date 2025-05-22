using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class Bench : MonoBehaviour
    {
        public bool interacted;
        public bool playerInZone;

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player"))
            {
                playerInZone = true;
            }
        }

        private void OnTriggerExit2D(Collider2D _other)
        {
            if (_other.CompareTag("Player"))
            {
                playerInZone = false;
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Interact") && playerInZone)
            {
                Debug.Log("Interacting with bench");
                interacted = true;
            }
        }
  }
}

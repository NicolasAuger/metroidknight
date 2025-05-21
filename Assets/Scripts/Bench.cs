using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class Bench : MonoBehaviour
    {

        public bool interacted;

        private void OnTriggerStay2D(Collider2D _other)
        {
            if (_other.CompareTag("Player") && Input.GetButtonDown("Interact"))
            {
                Debug.Log("Interacted with bench");
                interacted = true;
            }
        }
    }
}

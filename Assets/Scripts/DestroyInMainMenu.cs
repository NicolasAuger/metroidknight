using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight
{
    public class DestroyInMainMenu : MonoBehaviour
    {
        void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

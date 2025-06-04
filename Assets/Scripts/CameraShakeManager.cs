using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

namespace Metroknight
{
    public class CameraShakeManager : MonoBehaviour
    {
        public static CameraShakeManager Instance;
        [SerializeField] private float globalShakeForce = 1f;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void CameraShake(CinemachineImpulseSource impulseSource)
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }
    }
}

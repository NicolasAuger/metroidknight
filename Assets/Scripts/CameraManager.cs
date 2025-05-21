using UnityEngine;
using Cinemachine;

namespace Metroknight {
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;
        private CinemachineVirtualCamera currentCamera;
        private CinemachineFramingTransposer framingTransposer;
        public static CameraManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            for (int i = 0; i < allVirtualCameras.Length; i++)
            {
                if (allVirtualCameras[i].enabled)
                {
                    currentCamera = allVirtualCameras[i];
                    framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    break;
                }
            }
        }

        private void Start()
        {
            for (int i = 0; i < allVirtualCameras.Length; i++)
            {
                allVirtualCameras[i].Follow = PlayerController.Instance.transform;
            }
        }

        public void SwapCamera(CinemachineVirtualCamera _newCamera)
        {
            currentCamera.enabled = false;
            currentCamera = _newCamera;
            currentCamera.enabled = true;
        }
    }
}


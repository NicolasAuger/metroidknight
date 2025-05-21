using UnityEngine;
using Cinemachine;
using System.Collections;

namespace Metroknight {
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;
        private CinemachineVirtualCamera currentCamera;
        private CinemachineFramingTransposer framingTransposer;

        [Header("Y Damping Settings for Jump/Fall")]
        [SerializeField] private float panAmount = 0.1f;
        [SerializeField] private float panTime = 0.2f;
        public float playerFallSpeedThreshold = -10;
        public bool isLerpingYDamping = false;
        public bool hasLearpedYDamping = false;
        private float normalYDamp;

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

            normalYDamp = framingTransposer.m_YDamping;
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

        public IEnumerator LerpYDaming(bool _isPlayerFalling)
        {
            isLerpingYDamping = true;

            // Take start Y damp amount
            float _startYDamp = framingTransposer.m_YDamping;
            float _endYDamp = 0;

            // Determine the end Y damp amount
            if (_isPlayerFalling)
            {
                _endYDamp = panAmount;
                hasLearpedYDamping = true;
            }
            else
            {
                _endYDamp = normalYDamp;
            }

            // Lerp panAmount
            float _timer = 0;
            while (_timer < panTime)
            {
                _timer += Time.deltaTime;
                float _lerpedPanAmount = Mathf.Lerp(_startYDamp, _endYDamp, _timer / panTime);
                framingTransposer.m_YDamping = _lerpedPanAmount;
                yield return null;
            }
            isLerpingYDamping = false;
        }
    }
}


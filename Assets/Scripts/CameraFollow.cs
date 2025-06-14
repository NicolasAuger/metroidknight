using UnityEngine;

namespace Metroknight {
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                PlayerController.Instance.transform.position + offset,
                followSpeed
            );
        }
    }
}
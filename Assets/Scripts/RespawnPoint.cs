using UnityEngine;

namespace Metroknight
{
  public class RespawnPoint : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D _other)
    {
      if (_other.CompareTag("Player"))
      {
        GameManager.Instance.platformingRespawnPoint = transform.position;
      }
    }
  }
}

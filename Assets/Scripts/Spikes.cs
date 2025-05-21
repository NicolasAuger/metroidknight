using System.Collections;
using UnityEngine;

namespace Metroknight
{
  public class Spikes : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D _other)
    {
      if (_other.CompareTag("Player") && PlayerController.Instance.pState.alive)
      {
        StartCoroutine(RespawnPoint());
      }
    }

    IEnumerator RespawnPoint()
    {
      PlayerController.Instance.pState.cutscene = true;
      PlayerController.Instance.pState.invincible = true;
      PlayerController.Instance.rb.velocity = Vector2.zero;
      PlayerController.Instance.rb.gravityScale = 0;

      // Time.timeScale = 0;
      StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
      PlayerController.Instance.TakeDamage(1);
      yield return new WaitForSecondsRealtime(1f);
      PlayerController.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;
      StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
      yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
      PlayerController.Instance.pState.cutscene = false;
      PlayerController.Instance.pState.invincible = false;
      PlayerController.Instance.rb.gravityScale = 12f;

      // Time.timeScale = 1;
    }
  }
}

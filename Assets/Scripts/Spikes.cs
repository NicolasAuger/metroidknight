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

      if (GameManager.Instance.platformingRespawnPoint == null)
      {
        PlayerController.Instance.transform.position = new Vector2(GameManager.Instance.defaultRespawnPoint.transform.position.x, GameManager.Instance.defaultRespawnPoint.transform.position.y);
      }
      else
      {
        PlayerController.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;
      }
      StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
      yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
      PlayerController.Instance.rb.gravityScale = 12f;
      PlayerController.Instance.pState.cutscene = false;
      PlayerController.Instance.pState.invincible = false;

      // Time.timeScale = 1;
    }
  }
}

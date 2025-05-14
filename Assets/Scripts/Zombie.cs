using System.Collections;
using UnityEngine;

namespace Metroknight {
public class Zombie : Enemy
{
    void Start()
    {
      rb.gravityScale = 12f;
    }

    protected override void Awake() {
        base.Awake();
    }

    protected override void Update() {
        base.Update();
        // Own add-on, i added condition on player invincibility so that the enemy stop a little bit after touching the player
        if (!isRecoiling && !PlayerController.Instance.pState.invincible) {
            // Move towards the player following ennemy's y position
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce) {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }

    protected override void OnTriggerStay2D(Collider2D _other) {
        base.OnTriggerStay2D(_other);
        // if (_other.CompareTag("Player")) {
        //     rb.velocity = Vector2.zero;
        //     rb.gravityScale = 0f;
        // }
    }
  }
}

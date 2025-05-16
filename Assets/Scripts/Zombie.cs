using UnityEngine;

namespace Metroknight {
public class Zombie : Enemy
{
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }


    protected override void Update() {
        base.Update();
        // Own add-on, i added condition on player invincibility so that the enemy stop a little bit after touching the player
        if (!isRecoiling) {
            // Move towards the player following ennemy's y position
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce) {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
  }
}

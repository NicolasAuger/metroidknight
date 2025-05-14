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

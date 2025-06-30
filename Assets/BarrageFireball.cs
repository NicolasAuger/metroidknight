using UnityEngine;

namespace Metroknight
{
    public class BarrageFireball : MonoBehaviour
    {
        [SerializeField] Vector2 startForceMinMax;
        [SerializeField] float turnSpeed = 0.5f;

        Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.AddForce(transform.right * Random.Range(startForceMinMax.x, startForceMinMax.y), ForceMode2D.Impulse);
        }

        private void Update()
        {
            var _dir = rb.linearVelocity;

            if (_dir != Vector2.zero)
            {
                // Get the angle in radians and convert it to degrees
                float _targetAngle = Mathf.Atan2(_dir.y, Mathf.Abs(_dir.x)) * Mathf.Rad2Deg;

                if (_dir.x > 0)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, _targetAngle), turnSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, _targetAngle), turnSpeed);

                    // Rotate 180f on Y to face the player
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
            {
                _other.GetComponent<PlayerController>().TakeDamage(TheHollowKnight.Instance.damage);
                if (PlayerController.Instance.pState.alive)
                {
                    PlayerController.Instance.HitStopTime(0, 5, .5f);
                }
                Destroy(gameObject);
            }
            else if (_other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
            }
        }

    }
}

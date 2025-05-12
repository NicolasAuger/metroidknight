using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight {
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    private float xAxis;

    [Header("GroundCheck Settings")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    public static PlayerController Instance;

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
      } else {
        Instance = this;
      }
      
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Move();
        Flip();
        Jump();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void Move() {
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
        animator.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    public bool Grounded() {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckX, groundLayer) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckX, groundLayer)) {
            return true;
        } else {
            return false;
        }
    }

    void Flip() {
        // Flip the player using the Y euler rotation angle based on the xAxis
        if (xAxis > 0) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else if (xAxis < 0) {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void Jump() {
        // Variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // Jump only if the player is grounded
        if (Input.GetButtonDown("Jump") && Grounded()) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        animator.SetBool("Jumping", !Grounded());
    }

}
}
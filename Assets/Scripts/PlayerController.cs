using System.Collections;
using UnityEngine;

namespace Metroknight {
public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpBufferFrames;
    [SerializeField] private float coyoteTime;
    [SerializeField] private int maxAirJumps;
    private float jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    private int airJumpCounter = 0;
    [Space(5)]


    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]


    PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator animator;
    private float gravity;
    private float xAxis;
    private bool canDash = true;
    private bool dashed;
    private bool lookingLeft;

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
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        if (pState.dashing) return;
        Flip();
        Move();
        Jump();
        StartDash();
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
            lookingLeft = false;
        } else if (xAxis < 0) {
            transform.eulerAngles = new Vector3(0, 180, 0);
            lookingLeft = true;
        }
    }

    public void Jump() {
        // Variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        // Jump only if the player is grounded
        if (!pState.jumping) {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;
            } else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump")) {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }

        animator.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables() {
        if (Grounded()) {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        } else {
            // Time.deltaTime is the time between frames
            // So we decrease coyoteTimeCounter by 1 every second
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferFrames;
        } else {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
    }

    void StartDash() {
        if (Input.GetButtonDown("Dash") && canDash && !dashed) {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded()) {
            dashed = false;
        }
    }

    IEnumerator Dash() {
        canDash = false;
        pState.dashing = true;
        animator.SetBool("Dashing", true);
        rb.gravityScale = 0;
        if (lookingLeft) {
            rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
        } else {
            rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        }
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        animator.SetBool("Dashing", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

}
}
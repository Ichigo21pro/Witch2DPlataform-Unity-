using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoviminetoJugador : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public int maxJumps = 2;

    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 dashDirection;
    private bool isDashing = false;
    private bool canDash = true;
    private int remainingJumps;
    private float dashTimer;
    private float dashCooldownTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingJumps = maxJumps;
    }

    private void Update()
    {
        // Handle input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Jump") && remainingJumps > 0)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }

        // Handle dash cooldown
        if (!canDash)
        {
            dashCooldownTimer += Time.deltaTime;
            if (dashCooldownTimer >= dashCooldown)
            {
                canDash = true;
                dashCooldownTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            Dash();
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(targetVelocity.x))
        {
            rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, deceleration * Time.fixedDeltaTime);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        remainingJumps--;
    }

    private void StartDash()
    {
        isDashing = true;
        dashDirection = moveInput != Vector2.zero ? moveInput.normalized : new Vector2(transform.localScale.x, 0);
        canDash = false;
        dashTimer = 0f;
    }

    private void Dash()
    {
        rb.velocity = dashDirection * dashForce;
        dashTimer += Time.fixedDeltaTime;

        if (dashTimer >= dashDuration)
        {
            isDashing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset jumps when touching the ground
        if (collision.contacts[0].normal.y > 0.5f)
        {
            remainingJumps = maxJumps;
        }
    }
}

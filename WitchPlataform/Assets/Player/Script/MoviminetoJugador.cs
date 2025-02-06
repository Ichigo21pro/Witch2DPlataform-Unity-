using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Presets;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MoviminetoJugador : MonoBehaviour
{
    ////////////////////////////////////////
    // (movimiento AD)
    [Header("AD Move")]
    public float moveSpeed = 4f;
    public float runSpeed = 7f;
    bool running = false;
    private Rigidbody2D rb;
    Vector2 movement;
    ////////////////////////////////////////
    // (Salto space)
    [Header("Jump Move")]
    public float jumpForce = 7f;
    private bool isGrounded;
    // LayerMask para definir qué es el suelo
    public LayerMask groundLayer;
    // Transform para detectar el suelo
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    ////////////////////////////////////////
    // (Coyote Time)
    [Header("Coyote Time Settings")]
    public float tiempoCoyoteTime = 0.1f; // Tiempo en el que aún puede saltar después de caer
    [SerializeField] private bool coyoteTime = false; // Para saber si estamos dentro de coyote time o no
    [SerializeField] private float temporizadorCoyote; // temporizador
    ////////////////////////////////////////
    // (Jump buffer)
    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    ////////////////////////////////////////
    // (Gravity caida realista)
    [Header("Gravity Settings")]
    public float normalGravity = 2f;
    public float fallGravity = 4.5f; // gravedad aumentada al caer
    public float maxFallSpeed = -15f; // velocidad maxima de caida
    ////////////////////////////////////////

    private void Start()
    {
        ////////////////////////////////////////
        // (movimiento AD)
        // Obtener el componente Rigidbody2D del jugador
        rb = GetComponent<Rigidbody2D>();
        // (Gravity caida realista)
        // asignamos el valor al Rigidbody2D al iniciar el juego.
        rb.gravityScale = normalGravity;
    }

    private void Update()
    {
        ////////////////////////////////////////
        // asignamos la tecla para saber cuando esta corriendo (movimiento AD)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            running = false;
        }
        ////////////////////////////////////////
        // Input del salto (Salto space)
        // Comprobar si el jugador está tocando el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // (Coyote Time) - Mantiene un temporizador si dejó de tocar el suelo
        if (isGrounded)
        {
            coyoteTime = true;
            temporizadorCoyote = 0f;
        }
        if (!isGrounded && coyoteTime)
        {
            temporizadorCoyote += Time.deltaTime;
            if (temporizadorCoyote > tiempoCoyoteTime)
            {
                coyoteTime = false;
            }
        }
        ////////////////////////////////////////
        // detectamos y guardamos el salto antes de tocar el suelo (Jump buffer)
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else 
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        ////////////////////////////////////////
        // Input del salto (Salto space) con (Coyote Time) con modificacion de (Jump buffer)
        if (jumpBufferCounter>0f && (isGrounded || coyoteTime))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            temporizadorCoyote = 0f;

        }

        ////////////////////////////////////////
        // Ajuste de gravedad cuando esta ponsitivo y cunado empieza a caer (Gravity caida realista)
        if (rb.velocity.y > 0f) // Subiendo
        {
            rb.gravityScale = normalGravity;
        }
        else if (rb.velocity.y < 0f) // Cayendo
        {
            rb.gravityScale = fallGravity;
        }
        ////////////////////////////////////////
    }

    private void FixedUpdate()
    {
        ////////////////////////////////////////
        //Input del movimiento (movimiento AD)
        float ejeXMovimientoPlayer = Input.GetAxisRaw("Horizontal"); //movimiento Horizontal AD <- -> 
        // Movimiento en si (movimiento AD)
        if (running)
        {
            rb.velocity = new Vector2(ejeXMovimientoPlayer * runSpeed, rb.velocity.y);
            //rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        }
        else
        {
            rb.velocity = new Vector2(ejeXMovimientoPlayer * moveSpeed, rb.velocity.y);
        }
        ////////////////////////////////////////
        // Limitamos la velocidad de caida(Gravity caida realista)
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
        ////////////////////////////////////////
    }
    ////////////////////////////////////////
    ////////////////////////////////////////
    // Dibujar Gizmos para visualizar el groundCheckRadius en el editor
    public void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green; // Color del círculo
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }


}

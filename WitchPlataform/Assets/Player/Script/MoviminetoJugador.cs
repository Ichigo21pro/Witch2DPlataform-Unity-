using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Presets;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/*
Antes de nada se tiene que añadir dos cosas,
un emty object para que haga de groundCheck y un layer para lo que sera el suelo
*/

public class MoviminetoJugador : MonoBehaviour
{
    //referenciamos nuestras habilidades 
    public HabilidadesJugador HabilidadesJugador;
    ////////////////////////////////////////
    // (movimiento AD)
    [Header("AD Move")]
    public float moveSpeed = 4f;
    public float runSpeed = 7f;
    bool running = false;
    private float direccion;
    //pasamos valor de direccion
    public float direccionValor => direccion;
    //para que se pueda recoger desde otros scripts
    public Rigidbody2D rb { get; private set; }
    Vector2 movement;
    // Nueva variable para controlar si el jugador puede moverse
    public bool puedeMoverse = true;
    ////////////////////////////////////////
    // (Salto space)
    [Header("Jump Move")]
    public float jumpForce = 7f;
        //para que se pueda recoger desde otros scripts
    public bool isGrounded { get; private set; }
    // LayerMask para definir qué es el suelo
    public LayerMask groundLayer;
    // Transform para detectar el suelo
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    ////////////////////////////////////////
    // (Coyote Time)
    [Header("Coyote Time Settings")]
    public float tiempoCoyoteTime = 0.1f; // Tiempo en el que aún puede saltar después de caer
    private bool coyoteTime = false; // Para saber si estamos dentro de coyote time o no
    private float temporizadorCoyote; // temporizador
    ////////////////////////////////////////
    // (Jump buffer)
    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    ////////////////////////////////////////
    // (Gravity caida realista)
    [Header("Gravity Settings")]
    public float normalGravity = 2f ;
    public float normalGravityValue => normalGravity;
    public float fallGravity = 4.5f; // gravedad aumentada al caer
    public float maxFallSpeed = -15f; // velocidad maxima de caida
    ////////////////////////////////////////

    private void Awake()
    {
        ////////////////////////////////////////
        // (movimiento AD)
        // Obtener el componente Rigidbody2D del jugador
        rb = GetComponent<Rigidbody2D>();
        ////////////////////////////////////////
        // (Gravity caida realista)
        // asignamos el valor al Rigidbody2D al iniciar el juego.
        rb.gravityScale = normalGravity;
        ////////////////////////////////////////

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
        // (Salto space)
        // este if esta desde el script de habilidades diciendonos que mientras no se este dasheando se puede saltar
        if (!HabilidadesJugador.estaHaciendoDashValor)
        {
        Jump();
        }
    }

    private void FixedUpdate()
    {
        // Si no puede moverse, cancelamos el movimiento
        if (!puedeMoverse)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        ////////////////////////////////////////
        //Input del movimiento (movimiento AD)
        direccion = Input.GetAxisRaw("Horizontal"); //movimiento Horizontal AD <- -> 

        // Volteamos el personaje si cambia de dirección
        if (direccion != 0)
        {
            Flip();
        }
        // Movimiento en si (movimiento AD)
        if (running)
        {
            // este if esta desde el script de habilidades diciendonos que mientras no se este dasheando se puede correr
            if (!HabilidadesJugador.estaHaciendoDashValor)
            {
                rb.velocity = new Vector2(direccion * runSpeed, rb.velocity.y);
            }
        }
        else
        {
            // este if esta desde el script de habilidades diciendonos que mientras no se este dasheando se puede caminar
            if (!HabilidadesJugador.estaHaciendoDashValor)
            {
                rb.velocity = new Vector2(direccion * moveSpeed, rb.velocity.y);
            }
        }
        ////////////////////////////////////////
        // Limitamos la velocidad de caida(Gravity caida realista)
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
        ////////////////////////////////////////
    }



    // Función para voltear el personaje
    private void Flip()
    {
        // Verificamos si el personaje se está moviendo a la derecha o izquierda
        if (direccion > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Dirección normal
        }
        else if (direccion < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Volteamos en el eje X
        }
    }
    public void Flip(int direccion)
    {
        if (direccion > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direccion < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
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



    private void Jump()
    { 
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
        if (jumpBufferCounter > 0f && (isGrounded || coyoteTime))
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
        ////////////////////////////////////////}
    }
    }

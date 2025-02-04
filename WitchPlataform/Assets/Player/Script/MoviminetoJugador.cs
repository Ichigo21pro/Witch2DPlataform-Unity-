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
    public float moveSpeed = 0.2f;
    public float runSpeed = 0.4f;
    bool running = false;
    private Rigidbody2D rb;
    Vector2 movement;
    ////////////////////////////////////////
    // (Salto space)
    [Header("Jump Move")]
    public float jumpForce = 7f;
    // Variable para verificar si el jugador está en el suelo
    private bool isGrounded;
    // LayerMask para definir qué es el suelo
    public LayerMask groundLayer;
    // Transform para detectar el suelo
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    ////////////////////////////////////////

    private void Start()
    {
        // Obtener el componente Rigidbody2D del jugador
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) { running = false; }
        ////////////////////////////////////////
        // Input del salto (Salto space)
        // Comprobar si el jugador está tocando el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        ////////////////////////////////////////
    }

    private void FixedUpdate()
    {
        ////////////////////////////////////////
        //Input del movimiento (movimiento AD)
        float ejeXMovimientoPlayer = Input.GetAxisRaw("Horizontal"); //movimiento Horizontal AD <- -> 
        // Movimiento en si (movimiento AD)
        if (running == true)
        {
            rb.velocity = new Vector2(ejeXMovimientoPlayer * runSpeed, rb.velocity.y);
            //rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        }
        else {
            rb.velocity = new Vector2(ejeXMovimientoPlayer * moveSpeed, rb.velocity.y);
        }
        ////////////////////////////////////////
    }

}

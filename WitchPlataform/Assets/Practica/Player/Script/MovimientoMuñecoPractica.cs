using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovimientoMuñecoPractica : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Animator animator;
    ////////////////////////////////////////
    // (movimiento AD)
    [Header("AD Move")]
    [SerializeField] private float moveSpeed = 4f;
    private float direccion;

    ////////////////////////////////////////
    // (salto)
    [Header("Salto")]
    [SerializeField] private float jumpPower = 4f;
    private bool isGround;
    [SerializeField] private Vector3 dimensionesGroundCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask sueloLayer;
    ////////////////////////////////////////
    // (Coyote Time)
    [Header("Coyote Time Settings")]
    [SerializeField] private float tiempoCoyoteTime = 0.1f; // Tiempo en el que aún puede saltar después de caer
    private bool coyoteTime = false; // Para saber si estamos dentro de coyote time o no
    private float temporizadorCoyote; // temporizador
    ////////////////////////////////////////
    // (Jump buffer)
    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    ////////////////////////////////////////
    // (Gravity caida realista)
    [Header("Gravity Settings")]
    [SerializeField] private float normalGravity = 2f;
    [SerializeField] private float fallGravity = 4.5f; // gravedad aumentada al caer
    [SerializeField] private float maxFallSpeed = -15f; // velocidad maxima de caida
    ////////////////////////////////////////
    [Header("Jump Hold")]
    [SerializeField] private float jumpTimeMax = 0.35f; // Duración máxima del salto extendido
    private float jumpTimeCounter;
    private bool isJumping;
    ////////////////////////////////////////
    // (Wall Slide)
    [Header("Deslizamiento de pared")]
    public Transform wallCheck; // el detector de pared
    [SerializeField] private Vector3 dimensionesWallCheck;
    public LayerMask paredLayer;
    private bool tocandoPared;
    private bool estaDeslizando;
    public bool seEstaDeslizandoValue => estaDeslizando;
    public float wallSlideSpeed = 2f; // Velocidad de deslizamiento en la 
    ////////////////////////////////////////
    // (wall jump)
    [Header("Salto de pared")]
    [SerializeField] private float wallJumpForceX = 10f; // Fuerza horizontal del salto en la pared
    [SerializeField] private float wallJumpForceY = 15f; // Fuerza vertical del salto en la pared
    [SerializeField] private float tiempoSaltoPared;
    private bool puedeSaltarDePared; // Para evitar múltiples saltos seguidos
    ////////////////////////////////////////
    [Header("Correr")]
    [SerializeField] private bool isRunning; // Bool para saber cuando esta corriendo
    [SerializeField] private float runSpeed = 7f; // Velocidad al correr
    private float currentSpeed;
    ////////////////////////////////////////
    private bool isDead = false; // Para evitar múltiples llamadas a Die()


    ////////////////////////////////////////
    ////////////////////////////////////////
    private void Update()
    {
         
        if (!puedeSaltarDePared)
        {
            Moverse();
        }

        if (!estaDeslizando)
        {
            Saltar();
        }

        ////////////////////////////////////////
        // (Wall Slide)

        DetectarPared();
        GestionarWallSlide();



        ////////////////////////////////////////
        // (Wall Jump)    
        if (Input.GetButtonDown("Jump") && tocandoPared && estaDeslizando)
        {
            WallJump();
        }
        
        ////////////////////////////////////////


    }

    ////////////////////////////////////////
    ////////////////////////////////////////
    private void FixedUpdate()
    {
        //animacion para correr
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
        //slide wall
        SeDesliza();
    }
    ////////////////////////////////////////
    ////////////////////////////////////////

    // (movimiento AD)
    private void Moverse()
    {
        //Input del movimiento (movimiento AD)
        direccion = Input.GetAxisRaw("Horizontal"); //movimiento Horizontal AD <- -> 
        // si esta corriendo activamos bool
        if (isGround)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift) && direccion != 0;
            currentSpeed = isRunning ? runSpeed : moveSpeed;
        }
        // Volteamos el personaje si cambia de dirección
        if (direccion != 0)
        {
            Flip();
        }
        // Movimiento en si (movimiento AD)
        rb.velocity = new Vector2(direccion * currentSpeed, rb.velocity.y);
        animator.SetBool("isRunning", isRunning);
    }

    ////////////////////////////////////////
    // (salto)
    private void Saltar()
    {
        bool estabaEnElSuelo = isGround; // Guardamos el estado anterior
        isGround = Physics2D.OverlapBox(groundCheck.position, dimensionesGroundCheck, 0f, sueloLayer);

        if (isGround) { animator.SetBool("isGround", true); } else { animator.SetBool("isGround", false); }
        // Coyote Time
        if (isGround)
        {
            coyoteTime = true;
            temporizadorCoyote = 0f;
        }
        if (!isGround && coyoteTime)
        {
            temporizadorCoyote += Time.deltaTime;
            if (temporizadorCoyote > tiempoCoyoteTime)
            {
                coyoteTime = false;
            }
        }

        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Inicio del salto
        if (jumpBufferCounter > 0f && (isGround || coyoteTime))
        {
            isJumping = true; // Estamos saltando
            jumpTimeCounter = jumpTimeMax; // Reiniciamos el contador del salto extendido
            rb.velocity = new Vector2(direccion * currentSpeed, jumpPower);
            jumpBufferCounter = 0f;
            temporizadorCoyote = 0f;
            animator.SetBool("isJumping", true);
        }

        // Si mantiene el botón de salto y aún puede extenderlo
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpTimeCounter -= Time.deltaTime; // Reducimos el tiempo permitido de salto extendido
                animator.SetBool("isJumping", true);
            }
            else
            {
                isJumping = false; // Si ya no puede extender el salto, dejamos de forzarlo
            }
        }

        // Si suelta el botón de salto antes del límite, dejamos de extender el salto
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // Ajuste de gravedad para caída realista
        if (rb.velocity.y > 0f && !Input.GetButton("Jump")) // Si está subiendo pero soltó el botón
        {
            rb.gravityScale = normalGravity * 1.5f; // Aumentamos la gravedad para que no flote demasiado
        }
        else if (rb.velocity.y < 0f) // Cuando empieza a caer
        {
            rb.gravityScale = fallGravity;
        }

        // Si el personaje aterriza después de estar en el aire
        if (isGround && !estabaEnElSuelo)
        {
            animator.SetBool("isJumping", false);
        }
    }
    ////////////////////////////////////////
    ///
    private void DetectarPared()
    {
        tocandoPared = Physics2D.OverlapBox(wallCheck.position, dimensionesWallCheck, 0f, paredLayer);

    }
    // Maneja el deslizamiento en la pared
    private void GestionarWallSlide()
    {
        if (!isGround && tocandoPared && direccion != 0) //si no esta tocando suelo, esta tocando pared y la direccion es distinta de 0
        {
            estaDeslizando = true;
            animator.SetBool("isJumping", false);
            animator.SetBool("isWallSliding", true); // Activa animación de deslizarse
        }
        else
        {
            estaDeslizando = false;
            animator.SetBool("isWallSliding", false); // Desactiva animación de deslizarse
        }


    }
    private void SeDesliza()
    {
        if (estaDeslizando)
        {
            rb.velocity = new Vector2(rb.velocity.x, (Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue)));
            // Debug.Log("Me Deslizo a  = " + (Mathf.Clamp(jugador.rb.velocity.y, -wallSlideSpeed, float.MaxValue)));
            // no se desliza porque hay que crear un material 2d sin friccion
        }
    }


    // (Wall Jump)
    private void WallJump()
    {
        tocandoPared = false;
        rb.velocity = new Vector2(wallJumpForceX * -direccion, wallJumpForceY);
        //wait
        // Activa la animación de salto en la pared
        animator.SetBool("isWallJumping", true);
        //
        evitandoFlip = true; // Evita el Flip automático
        Flip(true); // Hace el Flip una vez al saltar de la pared
        StartCoroutine(CambioSaltoPared());
    }

    IEnumerator CambioSaltoPared()

    {
        puedeSaltarDePared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        puedeSaltarDePared = false;
        evitandoFlip = false; // Permite Flip nuevamente después del tiempo
        // Desactivar la animación de wall jump después de un corto tiempo
        animator.SetBool("isWallJumping", false);
    }
    ////////////////////////////////////////




    ////////////////////////////////////////

    // (FLIP)
    // Función para voltear el personaje
    private bool evitandoFlip = false; // Nueva variable para controlar el Flip
    private void Flip(bool forzarFlip = false)
    {
        if (!forzarFlip && evitandoFlip) return; // Evita cambiar la dirección si está bloqueado

        // Si forzamos el flip, simplemente invertimos la escala actual
        if (forzarFlip)
        {
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        }
        else
        {
            if (direccion > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direccion < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    ////////////////////////////////////////

    public void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow; // Color del Gizmo
            Gizmos.DrawWireCube(groundCheck.position, dimensionesGroundCheck);
            Gizmos.DrawWireCube(wallCheck.position, dimensionesWallCheck);
            Gizmos.color = Color.cyan; // Color diferente para la esquina


        }
    }


    //////////////////////////
    /// MUERTE
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trampa") && !isDead)
        {
            Die();
        }
    }
    private IEnumerator RestartAfterDeath()
    {
        yield return new WaitForSeconds(1f); // Espera la animación de muerte
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reinicia la escena
    }
    public void Die()
    {
        if (isDead) return; // Evita que la muerte se ejecute varias veces

        isDead = true;
        animator.SetTrigger("Die"); // Activa la animación de muerte

        // Detener otras animaciones
        animator.SetFloat("xVelocity", 0);
        animator.SetFloat("yVelocity", 0);
        animator.SetBool("isJumping", false);
        animator.SetBool("isGround", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isWallSliding", false);

        // Congelar movimiento del personaje
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        StartCoroutine(RestartAfterDeath()); // Esperar y reiniciar escena
    }

}

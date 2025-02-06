using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilidadesJugador : MonoBehaviour
{
    //cogemos el script de movimiento para coger datos
    private MoviminetoJugador jugador;

    ////////////////////////////////////////
    // (doble salto)
    [Header("Habilidad doble salto")]
    public bool dobleSaltoDesbloqueado = false;
    private bool puedeDobleSalto;
    ////////////////////////////////////////
    // (Dash)
    [Header("Habilidad Dash")]
    public bool dashDesbloqueado = false;
    public float dashSpeed = 20f; // Velocidad del dash
    public float dashDuration = 0.2f; // Duración del dash
    public float dashCooldown = 1f; // Tiempo de recarga del dash
    private bool puedeHacerDash = true;
    private bool estaHaciendoDash = false;
    //pasamos variables a player para controlar cuando se puede hacer el dash
    public bool estaHaciendoDashValor => estaHaciendoDash;
    ////////////////////////////////////////
    // (Wall Slide)
    [Header("Habilidad deslizamiento de pared")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask paredLayer;
    private bool tocandoPared;
    private bool estaDeslizando;
    public float wallSlideSpeed = 2f; // Velocidad de deslizamiento en la 
                                      ////////////////////////////////////////

    // (wall jump)
    [Header("Habilidad salto de pared")]
    public bool wallJumpDesbloqueado = false;
    public float wallJumpForceX = 10f; // Fuerza horizontal del salto en la pared
    public float wallJumpForceY = 15f; // Fuerza vertical del salto en la pared
    private bool puedeSaltarDePared = true; // Para evitar múltiples saltos seguidos


    private void Awake()
    {
        jugador = GetComponent<MoviminetoJugador>();
    }
    

    private void Update()
    {
        ////////////////////////////////////////
        // (doble salto)
        if (dobleSaltoDesbloqueado)
        {
            GestionarDobleSalto();
        }
        ////////////////////////////////////////
        // (Dash)
        if (dashDesbloqueado)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(EjecutarDash());
            }
        }
        ////////////////////////////////////////
        // (Wall Slide)
        DetectarPared();
        GestionarWallSlide();
        ////////////////////////////////////////
        // (Wall Jump)
        if (wallJumpDesbloqueado)
        {
            GestionarWallJump();
        }

    }


    ////////////////////////////////////////
    // (doble salto)
    private void GestionarDobleSalto()
    {
        // Si el jugador toca el suelo, restablece la habilidad de doble salto
        if (jugador.isGrounded)
        {
            puedeDobleSalto = true;
        }

        // Si el jugador presiona saltar y no está en el suelo, pero tiene doble salto disponible
        if (Input.GetButtonDown("Jump") && !jugador.isGrounded && puedeDobleSalto)
        {
            jugador.rb.velocity = new Vector2(jugador.rb.velocity.x, jugador.jumpForce);
            puedeDobleSalto = false; // Evita que pueda seguir haciendo doble salto
        }
    }
    ////////////////////////////////////////
    // (Dash)
    private IEnumerator EjecutarDash()
    {
        if (jugador.direccionValor != 0 && puedeHacerDash) 
        {
            estaHaciendoDash = true;
            puedeHacerDash = false;
            jugador.rb.gravityScale = 0f;
            jugador.rb.velocity = new Vector2(jugador.direccionValor * dashSpeed, 0f);
            yield return new WaitForSeconds(dashDuration);
            // Detener el dash y restaurar la velocidad normal del jugador
            estaHaciendoDash = false;
            jugador.rb.gravityScale = jugador.normalGravityValue;
            yield return new WaitForSeconds(dashCooldown); //coolDown de habilidad
            puedeHacerDash = true;
        }
        
    }
    ////////////////////////////////////////

    // Dibujar Gizmos para visualizar el wall CheckRadius en el editor
    public void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.green; // Color del círculo
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }
    ////////////////////////////////////////

    // (Wall Slide)
    // Detecta si el jugador está tocando una pared
    private void DetectarPared()
    {
        tocandoPared = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, paredLayer);
    }

    // Maneja el deslizamiento en la pared
    private void GestionarWallSlide()
    {
        if (tocandoPared && !jugador.isGrounded && jugador.rb.velocity.y <= 0)
        {
            estaDeslizando = true;
            jugador.rb.velocity = new Vector2(jugador.rb.velocity.x, -wallSlideSpeed);
        }
        else
        {
            estaDeslizando = false;
        }
    }

    ////////////////////////////////////////

    // (Wall Jump)
    private void GestionarWallJump()
    {
        // Verifica si está tocando la pared, no está en el suelo y presiona el botón de salto
        if (tocandoPared && !jugador.isGrounded && Input.GetButtonDown("Jump") && puedeSaltarDePared)
        {
            // Desactiva momentáneamente el control de movimiento para mayor fluidez
            StartCoroutine(WallJumpCoroutine());
        }
    }

    private IEnumerator WallJumpCoroutine()
    {
        puedeSaltarDePared = false; // Bloqueamos la capacidad de saltar nuevamente

        // Determinamos la dirección de la pared: si está tocando una pared a la derecha, se mueve hacia la izquierda, y viceversa
        int direccionSalto = jugador.direccionValor > 0 ? -1 : 1;

        // Flip: cambiamos la dirección del personaje independientemente de hacia donde esté mirando
        jugador.Flip(direccionSalto);

        // Aplicamos una fuerza en dirección opuesta a la pared
        jugador.rb.velocity = new Vector2(wallJumpForceX * direccionSalto, wallJumpForceY);

        // Desactivamos temporalmente la gravedad para mayor control durante el salto
        jugador.rb.gravityScale = 0.5f;

        yield return new WaitForSeconds(0.2f); // Pequeña pausa para que el salto tenga efecto

        // Restauramos la gravedad normal
        jugador.rb.gravityScale = jugador.normalGravityValue;

        // Breve cooldown para evitar múltiples saltos seguidos
        yield return new WaitForSeconds(0.2f);
        puedeSaltarDePared = true;
    }

    /*Desbloquear habilidades (doble salto)
    HabilidadJugador habilidades = jugador.GetComponent<HabilidadJugador>();
    habilidades.dobleSaltoDesbloqueado = true;
    habilidades.dashDesbloqueado = true; // Para desbloquear el Dash
    */
}

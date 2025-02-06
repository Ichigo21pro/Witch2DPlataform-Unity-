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



    /*Desbloquear habilidades (doble salto)
    HabilidadJugador habilidades = jugador.GetComponent<HabilidadJugador>();
    habilidades.dobleSaltoDesbloqueado = true;
    habilidades.dashDesbloqueado = true; // Para desbloquear el Dash
    */
}

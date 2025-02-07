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
    [SerializeField] private float dashDuration = 0.2f; // Duración del dash
    public float dashCooldown = 1f; // Tiempo de recarga del dash
    private bool puedeHacerDash = true;
    private bool estaHaciendoDash = false;
    //pasamos variables a player para controlar cuando se puede hacer el dash
    public bool estaHaciendoDashValor => estaHaciendoDash;
    ////////////////////////////////////////
    // (Wall Slide)
    [Header("Habilidad deslizamiento de pared")]
    public Transform wallCheck; // el detector de pared
    [SerializeField]private Vector3 dimensionesWallCheck;
    public LayerMask paredLayer;
    private bool tocandoPared;
    private bool estaDeslizando;
    public bool seEstaDeslizandoValue => estaDeslizando;
    public float wallSlideSpeed = 2f; // Velocidad de deslizamiento en la 
    ////////////////////////////////////////
    // (wall jump)
    [Header("Habilidad salto de pared")]
    public bool wallJumpDesbloqueado = false;
    [SerializeField] private float wallJumpForceX = 10f; // Fuerza horizontal del salto en la pared
    [SerializeField] private float wallJumpForceY = 15f; // Fuerza vertical del salto en la pared
    [SerializeField] private float tiempoSaltoPared;
    private bool puedeSaltarDePared ; // Para evitar múltiples saltos seguidos
    public bool SaltandoParedValue => puedeSaltarDePared;
    ////////////////////////////////////////

    private void Awake()
    {
        jugador = GetComponent<MoviminetoJugador>();
    }
    

    private void Update()
    {
        // Animaciones
        // animator.Setbool("Deslizando",estaDeslizando);
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
            if (Input.GetButtonDown("Jump")&& tocandoPared && estaDeslizando)
            {
                WallJump();
            }
            
        }

    }

    private void FixedUpdate()
    {
        ////////////////////////////////////////
        // (Wall Slide)
        SeDesliza();

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

    // Dibujar Gizmos para visualizar el wall CheckRadius en el editor
    public void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow; // Color del Gizmo
            Gizmos.DrawWireCube(wallCheck.position,dimensionesWallCheck);
        }
    }
    ////////////////////////////////////////

    // (Wall Slide)
    // Detecta si el jugador está tocando una pared
    private void DetectarPared()
    {
        tocandoPared = Physics2D.OverlapBox(wallCheck.position,dimensionesWallCheck,0f, paredLayer);
        
    }

    // Maneja el deslizamiento en la pared
    private void GestionarWallSlide()
    {
        if (!jugador.isGrounded && tocandoPared && jugador.direccionValor != 0) //si no esta tocando suelo, esta tocando pared y la direccion es distinta de 0
        {
            estaDeslizando = true;
        }
        else 
        {
            estaDeslizando = false;
        }

       
    }

    private void SeDesliza()
    {
        if (estaDeslizando)
        {
            
            jugador.rb.velocity = new Vector2(jugador.rb.velocity.x, (Mathf.Clamp(jugador.rb.velocity.y, -wallSlideSpeed, float.MaxValue)));
            // Debug.Log("Me Deslizo a  = " + (Mathf.Clamp(jugador.rb.velocity.y, -wallSlideSpeed, float.MaxValue)));
            // no se desliza porque hay que crear un material 2d sin friccion
        }
    }

    ////////////////////////////////////////

    // (Wall Jump)
    private void WallJump()
    {
        tocandoPared = false;
        jugador.rb.velocity = new Vector2(wallJumpForceX*-jugador.direccionValor, wallJumpForceY);
        //wait

        jugador.evitandoFlip = true; // Evita el Flip automático
        jugador.Flip(true); // Hace el Flip una vez al saltar de la pared
        StartCoroutine(CambioSaltoPared());
    }

    IEnumerator CambioSaltoPared()

    {
        puedeSaltarDePared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        puedeSaltarDePared = false;
        jugador.evitandoFlip = false; // Permite Flip nuevamente después del tiempo
    }
    /*private IEnumerator WallJumpCoroutine()
    {

    }*/

    /*Desbloquear habilidades (doble salto)
    HabilidadJugador habilidades = jugador.GetComponent<HabilidadJugador>();
    habilidades.dobleSaltoDesbloqueado = true;
    habilidades.dashDesbloqueado = true; // Para desbloquear el Dash
    */
}

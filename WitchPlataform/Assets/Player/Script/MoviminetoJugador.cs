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
    public Rigidbody2D rb;
    Vector2 movement;
    ////////////////////////////////////////

    private void Start()
    {
       
       
    }

    private void Update()
    {
        ////////////////////////////////////////
        // Input del movimiento (movimiento AD)
        movement.x = Input.GetAxisRaw("Horizontal"); //movimiento Horizontal AD <- -> 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) { running = false;}
        ////////////////////////////////////////
    }

    private void FixedUpdate()
    {
        ////////////////////////////////////////
        // Movimiento en si (movimiento AD)
        if (running == true) {
            Debug.Log("Corriendo");
            rb.MovePosition(rb.position + movement * runSpeed); 
        } 
        else {
            Debug.Log("Normal");
            rb.MovePosition(rb.position + movement * moveSpeed);
        }
        ////////////////////////////////////////
    }

}

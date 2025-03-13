using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint; // Punto de reaparición

    public void Die()
    {
        // Reiniciar la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Alternativamente, puedes mover al jugador al punto de reaparición:
        // transform.position = respawnPoint.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trampa")) // Si toca una trampa
        {
            Die();
        }
    }
   
}

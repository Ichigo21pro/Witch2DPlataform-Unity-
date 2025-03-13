using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraTrigger : MonoBehaviour
{
    public camaraFollow cameraFollow; // Referencia al script de la cámara
    public Vector3 newOffset = new Vector3(0, 5, -15); // Nueva posición de la cámara
    public float newFollowSpeed = 5f;
    public float newSmoothTime = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("hi");
            cameraFollow.AdjustCamera(newOffset, newFollowSpeed, newSmoothTime);
        }
    }
}

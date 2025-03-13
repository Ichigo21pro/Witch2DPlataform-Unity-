using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraTrigger : MonoBehaviour
{
    public camaraFollow cameraFollow; // Referencia al script de la c�mara
    public Vector3 newOffset = new Vector3(0, 5, -15); // Nueva posici�n de la c�mara
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

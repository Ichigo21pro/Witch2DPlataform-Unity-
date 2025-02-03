using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camaraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The object the camera will follow

    [Header("Camera Settings")]
    public float followSpeed = 5f; // Speed at which the camera follows the target
    public Vector3 offset = new Vector3(0, 2, -10); // Offset position of the camera relative to the target

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
}

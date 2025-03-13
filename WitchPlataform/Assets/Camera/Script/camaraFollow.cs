using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camaraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Camera Settings")]
    public float followSpeed = 7.5f;
    public float smoothTime = 0.15f;
    public Vector3 offset = new Vector3(0, 2, -10);

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }

    public void AdjustCamera(Vector3 newOffset, float newFollowSpeed, float newSmoothTime)
    {
        offset = newOffset;
        followSpeed = newFollowSpeed;
        smoothTime = newSmoothTime;
    }
}

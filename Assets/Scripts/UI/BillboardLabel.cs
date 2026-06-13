using UnityEngine;

// Makes a world-space label face the camera.
// Useful for shop signs, NPC name labels, fishing spot labels, and boss arena markers.
public class BillboardLabel : MonoBehaviour
{
    public Camera targetCamera;
    public bool lockYRotationOnly = true;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            return;
        }

        Vector3 directionToCamera = transform.position - targetCamera.transform.position;

        if (lockYRotationOnly)
        {
            directionToCamera.y = 0f;
        }

        if (directionToCamera.sqrMagnitude <= 0.001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(directionToCamera.normalized, Vector3.up);
    }
}

using UnityEngine;

// Placeholder movement for low-poly NPCs: rotate in place and optionally wander a little.
// Keep this subtle so NPCs feel alive without needing animation clips.
public class SimpleNPCIdleMotion : MonoBehaviour
{
    public bool rotateInPlace = true;
    public float rotationSpeed = 20f;
    public bool wander = false;
    public float wanderRadius = 1.5f;
    public float wanderSpeed = 0.75f;
    public float changeDirectionEvery = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float changeTimer;

    private void Start()
    {
        startPosition = transform.position;
        PickNewTarget();
    }

    private void Update()
    {
        if (rotateInPlace)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        if (!wander)
        {
            return;
        }

        changeTimer -= Time.deltaTime;
        if (changeTimer <= 0f || Vector3.Distance(transform.position, targetPosition) < 0.15f)
        {
            PickNewTarget();
        }

        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, wanderSpeed * Time.deltaTime);
        transform.position = nextPosition;
    }

    private void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
        changeTimer = changeDirectionEvery;
    }
}

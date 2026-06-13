using UnityEngine;

// Simple low-poly decoration motion for crystals, lanterns, portal bits, and signs.
public class VillageDecorationMotion : MonoBehaviour
{
    public bool spin = true;
    public Vector3 spinAxis = Vector3.up;
    public float spinSpeed = 30f;
    public bool bob = false;
    public float bobHeight = 0.15f;
    public float bobSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (spin)
        {
            transform.Rotate(spinAxis.normalized * spinSpeed * Time.deltaTime, Space.Self);
        }

        if (bob)
        {
            float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = startPosition + Vector3.up * offset;
        }
    }
}

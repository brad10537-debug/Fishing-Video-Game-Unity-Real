using UnityEngine;

// Optional helper for dropped fish/items that should bounce around as simple props.
// Add a Rigidbody and Collider to the same object, then enable physics in the Inspector.
[RequireComponent(typeof(Rigidbody))]
public class DroppedPhysicsItem : MonoBehaviour
{
    public string itemName = "Dropped Item";
    public bool enablePhysicsOnStart = true;
    public float spawnPopForce = 2f;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        body.isKinematic = !enablePhysicsOnStart;

        if (enablePhysicsOnStart && spawnPopForce > 0f)
        {
            body.AddForce(Vector3.up * spawnPopForce, ForceMode.Impulse);
        }
    }

    public void EnablePhysics()
    {
        body.isKinematic = false;
        Debug.Log(itemName + " physics enabled.");
    }
}

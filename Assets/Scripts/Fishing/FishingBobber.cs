using UnityEngine;

// Legacy bobber visual from the first fishing prototype.
// Keep this if you want to revisit physical casting visuals later.
public class FishingBobber : MonoBehaviour
{
    [Header("Visuals")]
    public Renderer bobberRenderer;
    public Color waitingColor = Color.white;
    public Color biteColor = Color.red;

    [Header("Bobbing")]
    public float bobHeight = 0.12f;
    public float bobSpeed = 3f;

    private Vector3 landedPosition;
    private bool hasLanded;
    private bool hasBite;

    private void Awake()
    {
        if (bobberRenderer == null)
        {
            bobberRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void Update()
    {
        if (!hasLanded)
        {
            return;
        }

        float biteMultiplier = hasBite ? 2f : 1f;
        float offset = Mathf.Sin(Time.time * bobSpeed * biteMultiplier) * bobHeight * biteMultiplier;
        transform.position = landedPosition + Vector3.up * offset;
    }

    public void LandAt(Vector3 position)
    {
        landedPosition = position;
        hasLanded = true;
        SetBite(false);
    }

    public void SetBite(bool biting)
    {
        hasBite = biting;

        if (bobberRenderer != null)
        {
            bobberRenderer.material.color = biting ? biteColor : waitingColor;
        }
    }
}

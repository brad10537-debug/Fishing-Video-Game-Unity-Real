using UnityEngine;

// Simple glow/color pulse for low-poly magical props.
// Works best on mushrooms, crystals, lantern glass, portal pieces, and boss markers.
[RequireComponent(typeof(Renderer))]
public class MaterialPulse : MonoBehaviour
{
    public Color baseColor = new Color(0.4f, 0.8f, 1f);
    public Color pulseColor = new Color(0.9f, 0.3f, 1f);
    public float pulseSpeed = 2f;
    [Range(0f, 1f)] public float pulseStrength = 0.5f;
    public bool useEmission = true;
    public float emissionIntensity = 1.5f;

    private Renderer targetRenderer;
    private Material runtimeMaterial;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            runtimeMaterial = targetRenderer.material;
        }
    }

    private void Update()
    {
        if (runtimeMaterial == null)
        {
            return;
        }

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f * pulseStrength;
        Color currentColor = Color.Lerp(baseColor, pulseColor, pulse);
        runtimeMaterial.color = currentColor;

        if (useEmission)
        {
            runtimeMaterial.EnableKeyword("_EMISSION");
            runtimeMaterial.SetColor("_EmissionColor", currentColor * emissionIntensity);
        }
    }
}

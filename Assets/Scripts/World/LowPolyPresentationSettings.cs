using UnityEngine;

// Optional scene helper for a bright low-poly prototype look.
// This uses built-in Unity lighting/render settings only: no custom shader required.
public class LowPolyPresentationSettings : MonoBehaviour
{
    public Camera mainCamera;
    public Light sunLight;
    public Color cameraBackground = new Color(0.48f, 0.8f, 1f);
    public Color ambientLight = new Color(0.72f, 0.76f, 0.82f);
    public Color fogColor = new Color(0.55f, 0.82f, 0.9f);
    public bool useFog = true;
    public float fogDensity = 0.012f;
    public float sunIntensity = 1.1f;

    private void Start()
    {
        ApplyPresentationSettings();
    }

    [ContextMenu("Apply Presentation Settings")]
    public void ApplyPresentationSettings()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = cameraBackground;
        }

        RenderSettings.ambientLight = ambientLight;
        RenderSettings.fog = useFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        if (sunLight != null)
        {
            sunLight.type = LightType.Directional;
            sunLight.intensity = sunIntensity;
            sunLight.shadows = LightShadows.Soft;
        }

        Debug.Log("Applied low-poly presentation settings.");
    }
}

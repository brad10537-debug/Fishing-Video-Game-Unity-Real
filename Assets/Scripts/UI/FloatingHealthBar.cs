using UnityEngine;
using UnityEngine.UI;

// Optional world-space name/health display for mobs, bosses, and important NPCs.
public class FloatingHealthBar : MonoBehaviour
{
    public LivingEntity target;
    public CharacterIdentity identity;
    public Text nameText;
    public Slider healthSlider;
    public Vector3 worldOffset = new Vector3(0f, 2.2f, 0f);
    public bool hideWhenFullHealth = false;
    public GameObject visualRoot;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (target == null)
        {
            target = GetComponentInParent<LivingEntity>();
        }

        if (identity == null)
        {
            identity = GetComponentInParent<CharacterIdentity>();
        }
    }

    private void OnEnable()
    {
        if (target != null)
        {
            target.EntityChanged += UpdateDisplay;
            UpdateDisplay(target);
        }
    }

    private void OnDisable()
    {
        if (target != null)
        {
            target.EntityChanged -= UpdateDisplay;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.transform.position + worldOffset;
        }

        if (mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }

    private void UpdateDisplay(LivingEntity entity)
    {
        if (nameText != null)
        {
            nameText.text = identity != null ? identity.displayName : entity.name;
        }

        if (healthSlider != null)
        {
            healthSlider.value = entity.GetHealthPercent();
        }

        if (hideWhenFullHealth)
        {
            SetVisualsVisible(entity.health < entity.maxHealth);
        }
    }

    private void SetVisualsVisible(bool isVisible)
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(isVisible);
            return;
        }

        if (nameText != null)
        {
            nameText.gameObject.SetActive(isVisible);
        }

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(isVisible);
        }
    }
}

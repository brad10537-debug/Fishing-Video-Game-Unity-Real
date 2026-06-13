using UnityEngine;

// Basic local health/stamina foundation for players, mobs, bosses, and NPCs.
// It does not control combat; it only stores and updates readable character stats.
public class LivingEntity : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina = 100f;
    public float staminaRegenRate = 15f;

    [Header("Movement")]
    public float movementSpeed = 5f;
    public float sprintSpeed = 8f;
    public float sprintStaminaCostPerSecond = 20f;

    [Header("Identity")]
    public int level = 1;
    public FactionTeam faction = FactionTeam.Neutral;
    public bool showFloatingHealthBar = true;

    public event System.Action<LivingEntity> EntityChanged;
    public event System.Action<LivingEntity> EntityDefeated;

    private void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
    }

    private void Update()
    {
        RegenerateStamina(Time.deltaTime);
    }

    public bool IsAlive()
    {
        return health > 0f;
    }

    public float GetHealthPercent()
    {
        return maxHealth <= 0f ? 0f : health / maxHealth;
    }

    public float GetStaminaPercent()
    {
        return maxStamina <= 0f ? 0f : stamina / maxStamina;
    }

    public bool TryUseStamina(float amount)
    {
        if (stamina < amount)
        {
            return false;
        }

        stamina -= amount;
        NotifyChanged();
        return true;
    }

    public void Damage(float amount)
    {
        bool wasAlive = IsAlive();
        health = Mathf.Clamp(health - Mathf.Max(0f, amount), 0f, maxHealth);
        Debug.Log(name + " health: " + health + " / " + maxHealth);
        NotifyChanged();

        if (wasAlive && !IsAlive())
        {
            Debug.Log(name + " was defeated.");
            EntityDefeated?.Invoke(this);
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + Mathf.Max(0f, amount), 0f, maxHealth);
        NotifyChanged();
    }

    private void RegenerateStamina(float deltaTime)
    {
        if (stamina >= maxStamina)
        {
            return;
        }

        stamina = Mathf.Clamp(stamina + staminaRegenRate * deltaTime, 0f, maxStamina);
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        EntityChanged?.Invoke(this);
    }
}

using UnityEngine;

// Beginner-friendly test button for knockback.
// Put this on a physics cube/capsule, look at it, and press E to push it.
public class KnockbackTestInteractable : MonoBehaviour, IInteractable
{
    public string displayName = "Wobbly Test Dummy";
    public KnockbackReceiver receiver;
    public float knockbackForce = 8f;
    public float damageAmount = 25f;

    private void Awake()
    {
        if (receiver == null)
        {
            receiver = GetComponent<KnockbackReceiver>();
        }
    }

    public string GetInteractPrompt()
    {
        LivingEntity target = receiver != null ? receiver.livingEntity : GetComponent<LivingEntity>();
        if (target != null && !target.IsAlive())
        {
            return displayName + " is defeated.";
        }

        return "Press E to knock back " + displayName;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (receiver == null || interactor == null)
        {
            return;
        }

        Vector3 direction = receiver.transform.position - interactor.transform.position;
        receiver.ApplyKnockback(direction, knockbackForce);

        LivingEntity target = receiver.livingEntity != null ? receiver.livingEntity : receiver.GetComponent<LivingEntity>();
        if (target != null)
        {
            target.Damage(damageAmount);
            string result = target.IsAlive()
                ? displayName + " takes " + Mathf.CeilToInt(damageAmount) + " damage."
                : displayName + " is defeated.";
            interactor.ShowStatus(result);
            return;
        }

        interactor.ShowStatus(displayName + " gets bonked by wizard force.");
    }
}

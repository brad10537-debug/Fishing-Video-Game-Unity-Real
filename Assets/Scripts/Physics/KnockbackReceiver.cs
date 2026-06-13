using System.Collections;
using UnityEngine;

// Prototype physics reaction for simple capsules, cubes, and props.
// This is not a full combat or ragdoll system; it just makes objects feel playful when pushed.
[RequireComponent(typeof(Rigidbody))]
public class KnockbackReceiver : MonoBehaviour
{
    public float defaultKnockbackForce = 8f;
    public float upwardForce = 2f;
    public float stunDuration = 1.25f;
    public bool tipOver = true;
    public bool recoverAfterStun = true;
    public LivingEntity livingEntity;

    private Rigidbody body;
    private Quaternion startingRotation;
    private Coroutine recoveryRoutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        startingRotation = transform.rotation;

        if (livingEntity == null)
        {
            livingEntity = GetComponent<LivingEntity>();
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        ApplyKnockback(direction, defaultKnockbackForce);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (body == null)
        {
            return;
        }

        Vector3 pushDirection = direction.normalized;
        pushDirection.y = 0f;

        body.isKinematic = false;
        body.AddForce((pushDirection * force) + (Vector3.up * upwardForce), ForceMode.Impulse);

        if (tipOver)
        {
            body.AddTorque(transform.right * force, ForceMode.Impulse);
        }

        Debug.Log(name + " received knockback.");

        if (recoveryRoutine != null)
        {
            StopCoroutine(recoveryRoutine);
        }

        if (recoverAfterStun)
        {
            recoveryRoutine = StartCoroutine(RecoverAfterStun());
        }
    }

    private IEnumerator RecoverAfterStun()
    {
        yield return new WaitForSeconds(stunDuration);

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, startingRotation.eulerAngles.y, 0f);

        Debug.Log(name + " recovered from knockback.");
    }
}

using UnityEngine;

// Simple placeholder mob behavior for the wild edge area.
// It wanders locally and can reuse KnockbackReceiver for playful physics reactions.
public class MobController : MonoBehaviour
{
    public CharacterIdentity identity;
    public LivingEntity livingEntity;
    public KnockbackReceiver knockbackReceiver;
    public float wanderRadius = 3f;
    public float wanderSpeed = 1.2f;
    public float waitAtPointTime = 1.5f;
    public bool disableWhenDefeated = true;

    private Vector3 spawnPosition;
    private Vector3 targetPosition;
    private float waitTimer;

    private void Awake()
    {
        if (identity == null)
        {
            identity = GetComponent<CharacterIdentity>();
        }

        if (livingEntity == null)
        {
            livingEntity = GetComponent<LivingEntity>();
        }

        if (knockbackReceiver == null)
        {
            knockbackReceiver = GetComponent<KnockbackReceiver>();
        }
    }

    private void Start()
    {
        spawnPosition = transform.position;
        PickNewTarget();
    }

    private void Update()
    {
        if (livingEntity != null && !livingEntity.IsAlive())
        {
            if (disableWhenDefeated)
            {
                enabled = false;
            }

            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, wanderSpeed * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            waitTimer = waitAtPointTime;
            PickNewTarget();
        }
    }

    private void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = spawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}

using UnityEngine;

// Prototype wild-edge mob area. Drop a simple mob prefab in the Inspector,
// then it spawns a few local wanderers when Play Mode starts.
public class MobSpawnArea : MonoBehaviour
{
    public string areaName = "Wild Edge";
    public GameObject mobPrefab;
    public int spawnCount = 3;
    public float spawnRadius = 6f;
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnMobs();
        }
    }

    public void SpawnMobs()
    {
        if (mobPrefab == null)
        {
            Debug.LogWarning(areaName + " needs a mob prefab before it can spawn mobs.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            GameObject mob = Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
            mob.name = mobPrefab.name + " " + (i + 1);
        }

        Debug.Log(areaName + " spawned " + spawnCount + " mobs.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

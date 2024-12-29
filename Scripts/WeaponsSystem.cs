using UnityEngine;
using System.Collections;

public class WeaponsSystem : MonoBehaviour
{
    [Header("Weapon Setup")]
    public GameObject[] rightWeapons;             // Array of weapon prefabs for the right
    public GameObject[] leftWeapons;              // Array of weapon prefabs for the left
    public Transform[] rightSpawnPoints;          // Array of spawn points on the right
    public Transform[] leftSpawnPoints;           // Array of spawn points on the left

    [Header("Spawn Control")]
    public float minSpawnInterval = 1f;           // Minimum time between spawns
    public float maxSpawnInterval = 3f;           // Maximum time between spawns
    public float spawnDuration = 10f;             // How long to keep spawning weapons
    public bool isSpawning = true;                // Toggle to start or stop spawning

    private float spawnTimer;

    void Start()
    {
        // Start the spawning coroutine
        if (isSpawning)
            StartCoroutine(SpawnWeapons());
    }

    // Coroutine to handle weapon spawning
    private IEnumerator SpawnWeapons()
    {
        float elapsedTime = 0f;

        while (elapsedTime < spawnDuration && isSpawning)
        {
            // Randomly decide whether to spawn from the right or the left
            bool spawnFromRight = Random.value > 0.5f;

            if (spawnFromRight && rightWeapons.Length > 0 && rightSpawnPoints.Length > 0)
            {
                // Pick a random weapon and spawn point from the right
                GameObject selectedWeapon = rightWeapons[Random.Range(0, rightWeapons.Length)];
                var indexPoint = Random.Range(0, rightSpawnPoints.Length);
                Transform selectedSpawnPoint = rightSpawnPoints[indexPoint];

                // Instantiate the weapon at the chosen spawn point
                GameObject weapon = Instantiate(selectedWeapon, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
                var script = weapon.gameObject.transform.GetChild(0).GetComponent<Indicator>();
                script.SetRotation(indexPoint);
               
            }
            else if (leftWeapons.Length > 0 && leftSpawnPoints.Length > 0)
            {
                // Pick a random weapon and spawn point from the left
                GameObject selectedWeapon = leftWeapons[Random.Range(0, leftWeapons.Length)];
                var indexPoint = Random.Range(0, leftSpawnPoints.Length);
                Transform selectedSpawnPoint = leftSpawnPoints[indexPoint];

                // Instantiate the weapon at the chosen spawn point
                GameObject weapon = Instantiate(selectedWeapon, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
                var script = weapon.gameObject.transform.GetChild(0).GetComponent<Indicator>();
                script.SetRotation(indexPoint+2);
            }

            // Wait for a random interval between spawns
            float nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(nextSpawnTime);

            // Increase elapsed time
            elapsedTime += nextSpawnTime;
        }
    }

    // Method to manually stop the spawning process
    public void StopSpawning()
    {
        isSpawning = false;
        StopCoroutine(SpawnWeapons());
    }

    // Method to manually start the spawning process
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnWeapons());
        }
    }
}

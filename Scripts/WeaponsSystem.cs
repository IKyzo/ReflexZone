    using UnityEngine;
    using System.Collections;


    public class WeaponsSystem : MonoBehaviour
    {
        [Header("Weapon Setup")]
        public GameObject[] weapons;                // Array of weapon prefabs
        public Transform[] spawnPoints;             // Array of spawn points

        [Header("Spawn Control")]
        public float minSpawnInterval = 1f;         // Minimum time between spawns
        public float maxSpawnInterval = 3f;         // Maximum time between spawns
        public float spawnDuration = 10f;           // How long to keep spawning weapons
        public bool isSpawning = true;              // Toggle to start or stop spawning

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

            while (elapsedTime < spawnDuration)
            {
                // Pick a random weapon
                GameObject selectedWeapon = weapons[Random.Range(0, weapons.Length)];

                // Pick a random spawn point
                Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // Instantiate the weapon at the chosen spawn point
                Instantiate(selectedWeapon, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

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

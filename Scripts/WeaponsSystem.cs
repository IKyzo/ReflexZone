using UnityEngine;
using TMPro;
using System.Collections;

public class WeaponsSystem : MonoBehaviour
{
    [Header("Weapon Setup")]
    public KeyboardLayoutWeapons currentLayout;
    public GameObject[] rightWeapons;             // Array of weapon prefabs for the right
    public GameObject[] leftWeapons;              // Array of weapon prefabs for the left
    public Transform[] rightSpawnPoints;          // Array of spawn points on the right
    public Transform[] leftSpawnPoints;           // Array of spawn points on the left

    [Header("Spawn Control")]
    public bool isSpawning = true;                // Toggle to start or stop spawning

    [Header("Score Control")]
    public TextMeshProUGUI scoreText;             // TextMeshProUGUI to read the player's score

    [SerializeField] private float minSpawnInterval;
    [SerializeField] private float maxSpawnInterval;



    public void SetKeyboardLayout(KeyboardLayoutWeapons newLayout) 
    {
        currentLayout = newLayout;
        LoadWeapons(); // Load the weapons from the new layout
    }

    public void LoadWeapons() 
    {
        if(currentLayout != null)
        {
            rightWeapons = currentLayout.rightWeapons;
            leftWeapons = currentLayout.leftWeapons;
        }
        else
        {
            Debug.LogError("No KeyboardLayoutWeapons assigned!");
        }
    }
    void Start()
    {
        // Start the spawning coroutine
        if (isSpawning)
            StartCoroutine(SpawnWeapons());
    }

    // Update spawn intervals based on the player's score
    private void UpdateSpawnIntervals()
    {
        int playerScore = GetPlayerScore();

        if (playerScore < 50)
        {
            minSpawnInterval = 1.5f;
            maxSpawnInterval = 3f;
        }
        else if (playerScore < 100)
        {
            minSpawnInterval = 1.25f;
            maxSpawnInterval = 2.5f;
        }
        else if (playerScore < 150)
        {
            minSpawnInterval = 1f;
            maxSpawnInterval = 2f;
        }
        else if (playerScore < 200)
        {
            minSpawnInterval = 0.75f;
            maxSpawnInterval = 1.5f;
        }
        else if (playerScore < 400)
        {
            minSpawnInterval = 0.5f;
            maxSpawnInterval = 1f;
        }
        else if (playerScore < 600)
        {
            minSpawnInterval = 0.4f;
            maxSpawnInterval = 1f;
        }
        else if (playerScore < 800)
        {
            minSpawnInterval = 0.3f;
            maxSpawnInterval = 1f;
        }
        else if (playerScore < 1000)
        {
            minSpawnInterval = 0.2f;
            maxSpawnInterval = 1f;
        }
        else
        {
            minSpawnInterval = 0.1f;
            maxSpawnInterval = 1f;
        }
    }

    // Get the player's score from the TextMeshProUGUI
    private int GetPlayerScore()
    {
        if (int.TryParse(scoreText.text, out int score))
        {
            return score;
        }
        return 0; // Default to 0 if parsing fails
    }

    // Coroutine to handle weapon spawning
    private IEnumerator SpawnWeapons()
    {
        while (isSpawning)
        {
            // Update spawn intervals based on the player's score
            UpdateSpawnIntervals();

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
                script.SetRotation(indexPoint + 2);
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
                script.SetRotation(indexPoint );
            }

            // Wait for a random interval between spawns
            float nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(nextSpawnTime);
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

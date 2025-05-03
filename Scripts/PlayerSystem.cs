using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSystem : MonoBehaviour
{
    // Input Keys : Q S D F J K L M
    [SerializeField] private LeaderboardManager leaderboardManager;

    [SerializeField] private FirebaseManager firebaseManager;
    public Animator animator;
    [SerializeField] private Image playerHealthBar;
    public float playerHealth = 100f;
    [SerializeField] private PostureSystem postureSystem;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private static readonly KeyCode[] InputKeys = 
    { 
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, 
        KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon 
    };

    private static readonly KeyCode[] LeftKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };
    private static readonly KeyCode[] RightKeys = { KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon };

    public float detectionRadius = 1f; // Radius for OverlapCircle
    public LayerMask weaponLayer;

    public ParticleSystem particleSystem;
    public Transform particleLeftPosition; // Position for particle effect when turning left
    public Transform particleRightPosition; // Position for particle effect when turning right

    
    public AudioSource slashSound;
    public AudioSource parrySound;
    public AudioSource hitSound;

    [SerializeField] private Animator deathAnimator;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private ParticleSystem playerParticleSystem;
    [SerializeField] private WeaponsSystem weaponsSystem;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    
    
    [SerializeField] private bool isDeflectSuccessful;
    private int score = 0;

    public bool playerState = true;
    public bool inputState = true;
    
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private GameObject restartText;

    [SerializeField] private float playerRunDuration;
    [SerializeField] private Transform playerResetPosition;
    [SerializeField] private Transform playerTargetPositionCenter;
    
    
    [SerializeField] private Animator healthAnimator;
    [SerializeField] private Animator postureAnimator;
    
    [SerializeField] private float pitchRange = 0.1f;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    private string playerName; // Default player name
    private int playerScore; // Default player score

    [SerializeField] private TextMeshProUGUI multiplierText;

    [SerializeField] private int deflectionScoreValue = 5; // Score value for a successful deflection

    private int scoreMultiplier = 0;

    [SerializeField] private Animator multiplierAnimator;
    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (inputState && playerState)
        {
            foreach (KeyCode key in InputKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    HandleDeflection(key);
                    HandleSpriteDirection(key);
                    animator.SetTrigger("Deflect");
                    slashSound.Play();
                
                    break;
                }
            }    
        }
        // Only check input if a relevant key is pressed
        
    }

    public void playerDamaged(float damage)
    {
        playerHealth -= damage;
        playerHealthBar.fillAmount = playerHealth / 100f;
        hitSound.Play();
        animator.SetTrigger("Hit");
        StartCoroutine(ApplyMultiplierAndReset());
        if (playerHealth <= 0)
        {
            playerState = false;
            weaponsSystem.isSpawning = false;
            StartCoroutine(gameManager.HideInstinct());
            animator.SetTrigger("Death");
            deathAnimator.SetTrigger("DeathSign");
            playerParticleSystem.Play();
            postureSystem.ReInitiatePosture();
            inputState = false;
            healthAnimator.SetTrigger("BringDown");
            postureAnimator.SetTrigger("BringDown");
            StartCoroutine(Restart());

            if(!gameManager.isGuest){
                StartCoroutine(SendPlayerScores());
            }
            
    

        }
    }
    private IEnumerator SendPlayerScores() {
        playerName = RemoveTrailingCharacter(playerNameText.text);
        playerScore = int.Parse(playerScoreText.text);
        yield return StartCoroutine(firebaseManager.PostScoreCoroutine(playerName, playerScore)); // Ensure this waits for Firebase to complete
        leaderboardManager.ShowLeaderboard(); 
    }
    private string RemoveTrailingCharacter(string playerName)
{
    // Check if the player name ends with "▌"
    if (playerName.EndsWith("▌"))
    {
        // Remove the "▌" character from the end of the string
        playerName = playerName.Substring(0, playerName.Length - 1);
    }

    return playerName;
}

   private void HandleDeflection(KeyCode key)
{
    // Get all weapons in the detection radius
    Collider2D[] hitWeapons = Physics2D.OverlapCircleAll(transform.position, detectionRadius, weaponLayer);
    isDeflectSuccessful = false;

    foreach (Collider2D weaponCollider in hitWeapons)
    {
        Weapon weapon = weaponCollider.GetComponent<Weapon>();
        if (weapon != null && weapon.deflectKey == key)
        {
            // Successful deflection
            Destroy(weapon.gameObject);
            //Debug.Log($"Deflected {weapon.name} with key {key}!");
            var weaponSpeed = Mathf.RoundToInt(weapon.speed / 2);
            isDeflectSuccessful = true;
            // Update score and animate the transition
            // *** Change here 
            int newScore = score + weaponSpeed;

            //currentStreakScore += deflectionScoreValue;
            scoreMultiplier++;
            multiplierAnimator.SetTrigger("multiply");
            UpdateMultiplierText();

            StartCoroutine(AnimateScoreChange(newScore));

            



            // Set particle position based on direction
            if (System.Array.Exists(RightKeys, k => k == key))
            {
                particleSystem.transform.position = particleRightPosition.position;
            }
            else if (System.Array.Exists(LeftKeys, k => k == key))
            {
                particleSystem.transform.position = particleLeftPosition.position;
            }

            // Play the particle effect
            particleSystem.Play();
            parrySound.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            parrySound.Play();
        
            postureSystem.UpdatePostureBar();
            postureSystem.AdjustPosture(isDeflectSuccessful);
            if(!isDeflectSuccessful){
                multiplierAnimator.SetTrigger("failed");
                StartCoroutine(ApplyMultiplierAndReset());
            }
            return;
        }
    }
    
    postureSystem.AdjustPosture(isDeflectSuccessful);
    // If no matching weapon found
    //Debug.Log($"Missed! Pressed {key} but no weapon to deflect.");
    postureSystem.UpdatePostureBar();
    multiplierAnimator.SetTrigger("failed");
    StartCoroutine(ApplyMultiplierAndReset());
    
}

private IEnumerator AnimateScoreChange(int targetScore)
{
    int startScore = score;
    score = targetScore;

    float duration = 0.5f; // Duration of the animation
    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        int currentScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
        scoreText.text = currentScore.ToString("D4");
        yield return null;
    }

    // Ensure the final score is displayed
    scoreText.text = targetScore.ToString("D4");
}

    
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString("D4");
    }
    

    private void HandleSpriteDirection(KeyCode key)
    {
        // Flip sprite based on the pressed key
        if (System.Array.Exists(RightKeys, k => k == key))
        {
            spriteRenderer.flipX = false; // Face left
        }
        else if (System.Array.Exists(LeftKeys, k => k == key))
        {
            spriteRenderer.flipX = true; // Face right
        }
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(5f);
        restartText.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.R))
        {
            
            yield return null;
        }
        restartText.SetActive(false);
        deathAnimator.SetTrigger("DeathReset");
        StartCoroutine(AnimateScoreChange(0));
        StartCoroutine(ResetPlayer());
        leaderboardManager.HideLeaderboard();


    }


    private IEnumerator ResetPlayer()
    {
        transform.position = playerResetPosition.position;
        HandleSpriteDirection(KeyCode.J);
        animator.SetTrigger("Run");
        
        var playerStartPosition = playerResetPosition.position;
        var elapsedTime = 0f;
        yield return new WaitForSeconds(1f);
        while (elapsedTime < playerRunDuration)
        {
            transform.position = Vector3.Lerp(playerStartPosition, playerTargetPositionCenter.position, elapsedTime / playerRunDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        
        animator.SetTrigger("Idle");
        StartCoroutine(gameManager.ShowInstinct());
        playerHealthBar.fillAmount = 100;
        playerHealth = 100f;
        healthAnimator.SetTrigger("BringUp");
        postureAnimator.SetTrigger("BringUp");
        
        inputState = true;
        playerState = true;
        
        yield return new WaitForSeconds(1f);
        weaponsSystem.StartSpawning();

    }
    private IEnumerator ApplyMultiplierAndReset()
    {
    if (scoreMultiplier > 1)
    {
        int bonusScore = deflectionScoreValue * scoreMultiplier;
        int newScore = score + bonusScore;
        StartCoroutine(AnimateScoreChange(newScore));
    }
    
    //currentStreakScore = 0;
    scoreMultiplier = 0;
    yield return new WaitForSeconds(0.5f); // Wait for the animation to finish
    UpdateMultiplierText();
    yield return null;
    }
    private void UpdateMultiplierText()
    {
    multiplierText.text =scoreMultiplier.ToString();
    }




    private void OnDrawGizmos()
    {
        // Visualize detection radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

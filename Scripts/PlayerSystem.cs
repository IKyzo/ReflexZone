using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSystem : MonoBehaviour
{
    // Input Keys : Q S D F J K L M
    public Animator animator;
    [SerializeField] private Image playerHealthBar;
    public float playerHealth = 100f;
    [SerializeField] private PostureSystem postureSystem;
    private SpriteRenderer spriteRenderer;

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
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (playerHealth <= 0)
        {
            playerState = false;
            weaponsSystem.isSpawning = false;
            animator.SetTrigger("Death");
            deathAnimator.SetTrigger("DeathSign");
            playerParticleSystem.Play();
            inputState = false;
            
        }
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
            int newScore = score + weaponSpeed;
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
            parrySound.Play();
            postureSystem.UpdatePostureBar();
            postureSystem.AdjustPosture(isDeflectSuccessful);
            return;
        }
    }
    postureSystem.AdjustPosture(isDeflectSuccessful);
    // If no matching weapon found
    Debug.Log($"Missed! Pressed {key} but no weapon to deflect.");
    postureSystem.UpdatePostureBar();
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

    private void OnDrawGizmos()
    {
        // Visualize detection radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

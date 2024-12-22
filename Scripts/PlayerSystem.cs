using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSystem : MonoBehaviour
{
    // Input Keys : Q S D F J K L M
    [SerializeField] private Animator animator;
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

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Only check input if a relevant key is pressed
        foreach (KeyCode key in InputKeys)
        {
            if (Input.GetKeyDown(key))
            {
                HandleDeflection(key);
                HandleSpriteDirection(key);
                animator.SetTrigger("Deflect");
                break;
            }
        }
    }

    public void playerDamaged(float damage)
    {
        playerHealth -= damage;
        playerHealthBar.fillAmount = playerHealth / 100f;
    }

    private void HandleDeflection(KeyCode key)
    {
        // Get all weapons in the detection radius
        Collider2D[] hitWeapons = Physics2D.OverlapCircleAll(transform.position, detectionRadius, weaponLayer);
        bool isDeflectSuccessful = false;

        foreach (Collider2D weaponCollider in hitWeapons)
        {
            Weapon weapon = weaponCollider.GetComponent<Weapon>();
            if (weapon != null && weapon.deflectKey == key)
            {
                // Successful deflection
                Destroy(weapon.gameObject);
                Debug.Log($"Deflected {weapon.name} with key {key}!");
                isDeflectSuccessful = true;
                postureSystem.UpdatePostureBar(10f);
                return;
            }
        }
        postureSystem.AdjustPosture(isDeflectSuccessful);
        // If no matching weapon found
        Debug.Log($"Missed! Pressed {key} but no weapon to deflect.");
        postureSystem.UpdatePostureBar(15f);
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
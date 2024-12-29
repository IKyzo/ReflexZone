using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform shinobiTargetTransform;
    [SerializeField] private float weaponDamage;
    public KeyCode deflectKey = KeyCode.F;
    public float speed = 5f;  // Speed of the weapon
    private Vector3 moveDirection;  // The direction the weapon will move
    [SerializeField] private GameObject indicator;
    [SerializeField] private bool weaponState = true;

    private void OnEnable()
    {
        // Set the initial move direction towards the player's position
        if (shinobiTargetTransform != null)
        {
            Vector3 targetPosition = shinobiTargetTransform.position;
            moveDirection = (targetPosition - transform.position).normalized; // Calculate and normalize the direction
        }
        else
        {
            // If no target is assigned, default to a forward direction
            moveDirection = Vector3.up;
        }

        int randomValue = Random.Range(15, 16); //30 max?
        speed = randomValue;

        // Rotate the weapon to face the initial direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            targetRotation *= Quaternion.Euler(0, 0, 180); // Adjust orientation if needed
            transform.rotation = targetRotation;
        }
    }

    private void Update()
    {
        // Move the weapon in the calculated direction
        if(weaponState){
            transform.position += moveDirection * speed * Time.deltaTime;    
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var script = other.GetComponent<PlayerSystem>();
            if (script != null && script.playerState)
            {
                script.playerDamaged(weaponDamage);
                Destroy(this.gameObject);
                
            }
            
        }

        if (other.CompareTag("Ground"))
        {
            weaponState = false;
        }
    }
}
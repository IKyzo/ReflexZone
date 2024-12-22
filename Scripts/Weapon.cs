using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform shinobiTargetTransform;
    [SerializeField] private float weaponDamage;
    public KeyCode deflectKey = KeyCode.F;
    [SerializeField] private float speed = 5f;  // Speed of the weapon
    private Vector3 targetPosition;  // The target position (player's position)

    private void OnEnable()
    {
        // Set the target position to the player's position
        targetPosition = shinobiTargetTransform.position;
    }

    private void Update()
    {
        // Move the weapon towards the target position using MoveTowards
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Get the direction to the target position
        Vector3 directionToTarget = targetPosition - transform.position;

        // If the direction is not zero (to avoid errors), rotate towards the target
        if (directionToTarget != Vector3.zero)
        {
            // Calculate the rotation we want to apply
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);

            // Apply a 180-degree offset to correct the orientation (if needed)
            targetRotation *= Quaternion.Euler(0, 0, 180);

            // Set the weapon's rotation directly towards the target
            transform.rotation = targetRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var script = other.GetComponent<PlayerSystem>();
            script.playerDamaged(weaponDamage);
        }
    }
}
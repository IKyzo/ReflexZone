using UnityEngine;

public class InstinctZone : MonoBehaviour
{
    // Rotation speed in degrees per second
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private bool rotateZ = true;
    [SerializeField] private bool rotateY = true;

    void Update()
    {
        // Rotate the object around its Y-axis
        if (rotateZ)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        if (rotateY)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);    
        }
        
    }
}

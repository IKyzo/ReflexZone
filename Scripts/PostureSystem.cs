using UnityEngine;
using UnityEngine.UI;

public class PostureSystem : MonoBehaviour
{
    public float currentPosture = 0f;       // Current posture level
    public float maxPosture = 100f;        // Maximum posture value
    public float decreaseRate = 0.001f;       // Rate at which posture decreases per second
    public float increaseOnDeflect = 15f;  // Increase for a successful deflect
    public float increaseOnMiss = 30f;    // Increase for a missed deflect
    public float recoveryRate = 2f;     // Rate at which posture recovers per second

    public Image postureBarL;  
    public Image postureBarR;  // Reference to UI bar image
    
    [SerializeField] private bool isKeyBeingPressed = false;  // Track if a key is being pressed

    private float recoveryCooldown = 0f;  // Timer to manage posture recovery
    
    [SerializeField] private Color postureColor1;
    [SerializeField] private Color postureColor2;
    [SerializeField] private Color postureColor3;

    void Update()
    {
        // If no key is pressed, slowly recover posture
        if (!isKeyBeingPressed && currentPosture < maxPosture)
        {
            if (currentPosture >= 0 && currentPosture < 50)
            {
                postureBarL.color = postureColor1;
                postureBarR.color = postureColor1;
            }
            if (currentPosture >= 50 && currentPosture < 85)
            {
                postureBarL.color = postureColor2;
                postureBarR.color = postureColor2;
            }
            if (currentPosture >= 85 && currentPosture < 101)
            {
                postureBarL.color = postureColor3;
                postureBarR.color = postureColor3;
            }
            // Gradually increase posture with the recovery rate
            currentPosture -= recoveryRate * Time.deltaTime;
            currentPosture = Mathf.Clamp(currentPosture, 0, maxPosture); // Ensure posture stays within bounds
        }

        // Update the posture bar to reflect current posture
        UpdatePostureBar(0f);
    }

    public void AdjustPosture(bool isSuccessfulDeflect)
    {
        // Increase posture based on deflect success
        if (isSuccessfulDeflect)
        {
            currentPosture += increaseOnDeflect;
        }
        else
        {
            currentPosture += increaseOnMiss;
        }

        // Clamp posture within bounds
        currentPosture = Mathf.Clamp(currentPosture, 0, maxPosture);

        // Reset the recovery cooldown whenever posture changes
        recoveryCooldown = 0f;

        // Update the posture bar to reflect the change
        UpdatePostureBar(0f);
    }

    public void UpdatePostureBar(float damage)
    {
        // Set bar fill amount (normalized value between 0 and 1)
        float normalizedPosture = currentPosture / maxPosture;

        // Adjust the fill to grow from the center
        postureBarL.fillAmount = normalizedPosture;
        postureBarR.fillAmount = normalizedPosture;
    }

    public void OnKeyPress()
    {
        isKeyBeingPressed = true;
    }

    public void OnKeyRelease()
    {
        isKeyBeingPressed = false;
    }
}

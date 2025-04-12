using UnityEngine;

public class SecretCodeListener : MonoBehaviour
{
    
    private int pressCount = 0;
    private float lastPressTime = 0f;
    public float resetTime = 2f; // Reset if they take longer than 2 seconds between presses

    [SerializeField] private GameObject specialStars;

    void Update()
    {
        if(!specialStars.activeSelf){
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) // Detect when "8" is pressed
        {
            if (Time.time - lastPressTime > resetTime)
            {
                pressCount = 0; // Reset if they waited too long
            }

            pressCount++;
            lastPressTime = Time.time;

            if (pressCount == 8)
            {
                ActivateSpecialFeature();
                pressCount = 0; // Reset so they can do it again later
            }
        }
        } // If the special stars are already active, ignore further presses
        
    }

    void ActivateSpecialFeature()
    {
        specialStars.SetActive(true); // Activate the special stars
        Debug.Log("🎉 Special Feature Activated! 🎉");
        // Add whatever special effect you want here!
    }
}

using UnityEngine;

public class Testing : MonoBehaviour
{
    private bool isTimeFrozen = false;

    void Update()
    {
        // Freeze time when the player presses "F"
        if (Input.GetKeyDown(KeyCode.F))
        {
            FreezeTime();
        }

        // Unfreeze time when the player presses "R"
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnfreezeTime();
        }
    }

    void FreezeTime()
    {
        if (!isTimeFrozen)
        {
            Time.timeScale = 0f; // Freeze time
            isTimeFrozen = true;
        }
    }

    void UnfreezeTime()
    {
        if (isTimeFrozen)
        {
            Time.timeScale = 1f; // Resume time
            isTimeFrozen = false;
        }
    }
    
    
    
}

using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI mainTimerText;  // Reference for hours:minutes:seconds
    public TextMeshProUGUI millisecondsText;  // Reference for milliseconds
    private float elapsedTime;       // Keeps track of time in seconds
    private bool isRunning = true;   // Controls whether the timer is running

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            // Calculate hours, minutes, seconds, and milliseconds
            int hours = Mathf.FloorToInt(elapsedTime / 3600);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            int milliseconds = Mathf.FloorToInt((elapsedTime % 1) * 1000);

            // Update the main timer (hours:minutes:seconds)
            mainTimerText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

            // Update the milliseconds
            millisecondsText.text = $":{milliseconds:D3}";
        }
    }

    // Optional: Methods to control the timer
    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        mainTimerText.text = "00:00:00";
        millisecondsText.text = ":000";
    }
}
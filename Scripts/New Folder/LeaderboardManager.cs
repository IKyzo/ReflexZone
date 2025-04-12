using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class LeaderboardManager : MonoBehaviour
{
    public FirebaseManager firebaseManager;
    public List<TextMeshProUGUI> leaderboardEntries; // Assign in inspector

    [SerializeField] private CanvasGroup canvasGroup;

    //public static string deviceID;
    void  Start()
    {
        // if(PlayerPrefs.HasKey("deviceID"))
        // {
        //     deviceID = PlayerPrefs.GetString("deviceID");
        // }
        // else
        // {
        //     deviceID = System.Guid.NewGuid().ToString();
        //     PlayerPrefs.SetString("deviceID", deviceID);
        // }
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        StartCoroutine(WaitAndLoad());
        //StartCoroutine(firebaseManager.GetScores(OnScoresLoaded));
    }


    private IEnumerator WaitAndLoad() {
        // Wait until FirebaseManager is ready
        yield return new WaitUntil(() => firebaseManager != null && firebaseManager.IsReady);

        StartCoroutine(firebaseManager.GetScores(OnScoresLoaded));
    }

    private void OnScoresLoaded(Dictionary<string, FirebaseManager.ScoreEntry> scores)
    {
        if (scores == null)
        {
            Debug.LogError("Failed to load scores.");
            return;
        }

        var sortedScores = scores.Values.OrderByDescending(entry => entry.score).ToList();

        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            if (i < sortedScores.Count)
            {
                leaderboardEntries[i].text = $"{i + 1}. {sortedScores[i].name}: {sortedScores[i].score}";
            }
            else
            {
                leaderboardEntries[i].text = $"{i + 1}. ---";
            }
        }
    }


    public void ShowLeaderboard()
    {
        StartCoroutine(WaitAndUpdateLeaderboard());
    }
    public void HideLeaderboard()
    {
        StartCoroutine(FadeCanvas(0f, 2f)); // Fade out the leaderboard
    }

    private IEnumerator WaitAndUpdateLeaderboard()
{
    // Wait until the leaderboard has been updated
    yield return StartCoroutine(firebaseManager.GetScores(OnScoresLoaded));

    // After data is loaded, show the leaderboard
    StartCoroutine(FadeCanvas(1f, 2f)); // Fade in the leaderboard
}

    private IEnumerator FadeCanvas(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }   
}

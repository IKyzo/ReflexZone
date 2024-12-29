using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PostureSystem : MonoBehaviour
{
    public float currentPosture = 0f;       // Current posture level
    public float maxPosture = 101f;        // Maximum posture value
    public float decreaseRate = 0.001f;    // Rate at which posture decreases per second
    public float increaseOnDeflect = 15f; // Increase for a successful deflect
    public float increaseOnMiss = 30f;    // Increase for a missed deflect
    public float recoveryRate = 2f;       // Rate at which posture recovers per second
    public float recoveryDelay = 1.5f;    // Delay before starting posture recovery

    public Image postureBarL;
    public Image postureBarR; // Reference to UI bar image

    private Coroutine recoveryCoroutine;  // Reference to the recovery coroutine

    [SerializeField] private bool isKeyBeingPressed = false; // Track if a key is being pressed
    private float recoveryCooldown = 0f; // Timer to manage posture recovery
    [SerializeField] private Color postureColor1;
    [SerializeField] private Color postureColor2;
    [SerializeField] private Color postureColor3;
    [SerializeField] private bool postureBroken = false;
    [SerializeField] private Animator postureAnimator;
    [SerializeField] private bool resetControl = true;

    [SerializeField] private PlayerSystem player;
    [SerializeField] private bool unbreakablePosture = false;

    void Update()
    {
        if (!isKeyBeingPressed && !postureBroken)
        {
            UpdatePostureColors();

            if (currentPosture >= maxPosture && resetControl)
            {
                if (!unbreakablePosture)
                {
                    player.animator.SetTrigger("Exhaust");
                    player.inputState = false;
                    StartCoroutine(ResetPosture());
                    resetControl = false;
                    postureBroken = true;
                }
            }

            if (currentPosture < maxPosture)
            {
                currentPosture -= recoveryRate * Time.deltaTime;
                currentPosture = Mathf.Clamp(currentPosture, 0, maxPosture);
            }
        }

        UpdatePostureBar();
    }

    private void UpdatePostureColors()
    {
        if (currentPosture < 50)
        {
            postureBarL.color = postureColor1;
            postureBarR.color = postureColor1;
        }
        else if (currentPosture < 85)
        {
            postureBarL.color = postureColor2;
            postureBarR.color = postureColor2;
        }
        else if (currentPosture < maxPosture)
        {
            postureBarL.color = postureColor3;
            postureBarR.color = postureColor3;
        }
    }

    public void AdjustPosture(bool isSuccessfulDeflect)
    {
        if (isSuccessfulDeflect)
        {
            currentPosture += increaseOnDeflect;
            if (currentPosture >= maxPosture)
            {
                unbreakablePosture = true;
                currentPosture = maxPosture;
            }
        }
        else
        {
            currentPosture += increaseOnMiss;
            if (currentPosture >= maxPosture)
            {
                unbreakablePosture = false;
            }
        }

        currentPosture = Mathf.Clamp(currentPosture, 0, maxPosture);

        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
        }
        recoveryCoroutine = StartCoroutine(DelayedPostureRecovery());

        UpdatePostureBar();
    }

    private IEnumerator DelayedPostureRecovery()
    {
        yield return new WaitForSeconds(recoveryDelay);

        while (currentPosture > 0 && !isKeyBeingPressed && !postureBroken)
        {
            currentPosture -= recoveryRate * Time.deltaTime;
            currentPosture = Mathf.Clamp(currentPosture, 0, maxPosture);
            UpdatePostureBar();
            yield return null;
        }
    }

    public void UpdatePostureBar()
    {
        float normalizedPosture = currentPosture / maxPosture;
        postureBarL.fillAmount = normalizedPosture;
        postureBarR.fillAmount = normalizedPosture;
    }

    private IEnumerator ResetPosture()
    {
        postureBarL.fillAmount = 1f;
        postureBarR.fillAmount = 1f;
        float colorTransitionDuration = 0.3f;
        float fillReductionDuration = 0.2f;
        Color startColor = new Color(220 / 255f, 50 / 255f, 50 / 255f);
        Color endColor = new Color(255 / 255f, 130 / 255f, 130 / 255f);

        for (int i = 0; i < 4; i++)
        {
            yield return ColorTransition(postureBarL, postureBarR,
                                         i % 2 == 0 ? startColor : endColor,
                                         i % 2 == 0 ? endColor : startColor,
                                         colorTransitionDuration);
        }

        float elapsedTime = 0f;
        while (elapsedTime < fillReductionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fillReductionDuration;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            postureBarL.color = currentColor;
            postureBarR.color = currentColor;
            postureBarL.fillAmount = Mathf.Lerp(1f, 0f, t);
            postureBarR.fillAmount = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        postureBarL.fillAmount = 0f;
        postureBarR.fillAmount = 0f;
        postureBroken = false;
        resetControl = true;
        currentPosture = 0;
        player.inputState = true;
    }

    private IEnumerator ColorTransition(Image barL, Image barR, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.PingPong(elapsedTime, duration) / duration;
            Color currentColor = Color.Lerp(startColor, endColor, t);

            barL.color = currentColor;
            barR.color = currentColor;

            yield return null;
        }
    }
}

using System.Collections;
using UnityEngine;

public class ShadowsManager : MonoBehaviour
{
    public SpriteRenderer background;
    public GameObject shadow1;
    public GameObject shadow2;
    public GameObject shadow3;
    public GameObject shadow4;

    public Transform shadowposition1;
    public Transform shadowposition2;
    public Transform shadowposition3;
    public Transform shadowposition4;

    public float fadeDuration = 1f; // Duration of the fade effect
    public float moveDuration = 1f; // Duration of the shadow translation

    private bool isFading = false;
    public Color targetColor;


    public GameObject HealthBar;
    public GameObject DeflectBar;
    public GameObject Weapons;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFading)
        {
            StartCoroutine(FadeBackgroundAndMoveShadows());
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            HealthBar.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            DeflectBar.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Weapons.SetActive(true);
        }
        
    }

    private IEnumerator FadeBackgroundAndMoveShadows()
    {
        isFading = true;

        // Fade the background color
        Color originalColor = background.color;
         // Darker tone
        float fadeTime = 0f;

        while (fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;
            background.color = Color.Lerp(originalColor, targetColor, fadeTime / fadeDuration);
            yield return null;
        }
        background.color = targetColor;

        // Move all shadows simultaneously
        Coroutine moveShadow1 = StartCoroutine(MoveShadow(shadow1, shadowposition1.position));
        Coroutine moveShadow2 = StartCoroutine(MoveShadow(shadow2, shadowposition2.position));
        Coroutine moveShadow3 = StartCoroutine(MoveShadow(shadow3, shadowposition3.position));
        Coroutine moveShadow4 = StartCoroutine(MoveShadow(shadow4, shadowposition4.position));

        // Wait for all shadows to finish moving
        yield return moveShadow1;
        yield return moveShadow2;
        yield return moveShadow3;
        yield return moveShadow4;

        isFading = false;
    }

    private IEnumerator MoveShadow(GameObject shadow, Vector3 targetPosition)
    {
        Vector3 originalPosition = shadow.transform.position;
        float moveTime = 0f;

        while (moveTime < moveDuration)
        {
            moveTime += Time.deltaTime;
            shadow.transform.position = Vector3.Lerp(originalPosition, targetPosition, moveTime / moveDuration);
            yield return null;
        }
        shadow.transform.position = targetPosition;
    }
}

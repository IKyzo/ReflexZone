using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject mainCamera;

    [SerializeField]
    private Vector3 logoPosition;
    
    [SerializeField]
    private Vector3 gamePosition;

    [SerializeField] private float logoShowcaseDuration;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float linesDuration;
    [SerializeField] private float instinctDuration;
    
    [SerializeField] private Animator logoAnimator;
    
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerSystem playerSystem;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Vector3 playerTargetPosition;
    [SerializeField] private float playerRunDuration;

    [SerializeField] private List<SpriteRenderer> instinctSprites;
    
    [SerializeField] private Animator healthAnimator;
    [SerializeField] private Animator postureAnimator;
    
    [SerializeField] private List<RectTransform> indicatorsTexts;
    [SerializeField] private List<RectTransform> indicatorsRects;
    

    [SerializeField] private List<float> widths;
    
    [SerializeField] private float yOffset = 100f;

    [SerializeField] private float durationText;
    
    
    [SerializeField] private Animator inputAnimator;
    [SerializeField] private Animator keysAnimator;
    
    [SerializeField] private GameObject weapon;
    
    [SerializeField] private GameObject pressKey;
    
    [SerializeField] private GameObject weapons;

    [SerializeField] private Animator scoreAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Awake()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 30; // Limit frame rate in the Editor
#else
    Application.targetFrameRate = -1; // No limit for builds
#endif
    }

    
    
    void Start()
    {
        StartCoroutine(Intro());
        playerSystem.inputState = false;
    }

    private IEnumerator Intro()
    {
        Vector3 startPosition = mainCamera.transform.position; // Initial position
        Vector3 targetPosition = logoPosition;   // Target position
                                        // Duration of movement

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        mainCamera.transform.position = targetPosition;
        logoAnimator.SetTrigger("ShowLogo");// Ensure it reaches the exact target position
        yield return new WaitForSeconds(logoShowcaseDuration);

        // Move to another position
        Vector3 secondPosition = gamePosition;
        elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(targetPosition, secondPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = secondPosition;
        StartCoroutine(StartGame());
        
    }

    private IEnumerator StartGame()
    {
        playerAnimator.SetTrigger("Run");
        var playerStartPosition = player.transform.position;
        var elapsedTime = 0f;
        while (elapsedTime < playerRunDuration)
        {
            player.transform.position = Vector3.Lerp(playerStartPosition, playerTargetPosition, elapsedTime / playerRunDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        
        playerAnimator.SetTrigger("Idle");
        
        healthAnimator.SetTrigger("BringUp");
        postureAnimator.SetTrigger("BringUp");
       // Time in seconds for the alpha transition
        float timeElapsed = 0f;

        // Get the current color and set the starting alpha to 0
        Color instinctColor = instinctSprites[0].color;
        Color instinctColor2 = instinctSprites[1].color; 
        Color instinctColorx = instinctSprites[0].color;
        Color instinctColor2x = instinctSprites[1].color; 
        
        
        instinctColor.a = 0f;  // Start with alpha 0
        instinctColor2.a = 0f; 
        instinctColorx.a = 0f;  // Start with alpha 0
        instinctColor2x.a = 0f; 
        
        
        
        
        instinctSprites[0].color = instinctColor;
        instinctSprites[1].color = instinctColor2;
        instinctSprites[2].color = instinctColorx;
        instinctSprites[3].color = instinctColor2x;
        
        // Lerp the alpha from 0 to 1 over time
        while (timeElapsed < instinctDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / instinctDuration);
            float alpha2 = Mathf.Lerp(0f, 1f, timeElapsed / instinctDuration);
            float alphax = Mathf.Lerp(0f, 0.25f, timeElapsed / instinctDuration);
            float alpha2x = Mathf.Lerp(0f, 0.25f, timeElapsed / instinctDuration);
            
            
            instinctColor.a = alpha;
            instinctColor2.a = alpha2;
            instinctColorx.a = alphax;
            instinctColor2x.a = alpha2x;
          
            instinctSprites[0].color = instinctColor;
            instinctSprites[1].color = instinctColor2;
            instinctSprites[2].color = instinctColorx;
            instinctSprites[3].color = instinctColor2x;
            
            
            yield return null;
        }

        instinctColor.a = 1f;
        instinctColor2.a = 0.25f;
        instinctSprites[0].color = instinctColor;
        instinctSprites[1].color = instinctColor2;
        instinctSprites[2].color = instinctColor;
        instinctSprites[3].color = instinctColor2;
    
        
        
         // Time in seconds for the alpha transition
         timeElapsed = 0f;
        
        while (timeElapsed < durationText)
        {
            timeElapsed += Time.deltaTime;
            float width  = Mathf.Lerp(0f, widths[0], timeElapsed / durationText);
            float width1  = Mathf.Lerp(0f, widths[1], timeElapsed / durationText);
            float width2  = Mathf.Lerp(0f, widths[2], timeElapsed / durationText);
            float width3  = Mathf.Lerp(0f, widths[3], timeElapsed / durationText);
            float width4  = Mathf.Lerp(0f, widths[4], timeElapsed / durationText);
            float width5  = Mathf.Lerp(0f, widths[5], timeElapsed / durationText);


            // Set the new width
            Vector2 size = indicatorsRects[0].sizeDelta;  // Get current size
            size.x = width;  // Set the new width
            indicatorsRects[0].sizeDelta = size;  // Apply the new size
            Vector2 size1 = indicatorsRects[1].sizeDelta;  // Get current size
            size1.x = width1;  // Set the new width
            indicatorsRects[1].sizeDelta = size1;
            Vector2 size2 = indicatorsRects[2].sizeDelta;  // Get current size
            size2.x = width2;  // Set the new width
            indicatorsRects[2].sizeDelta = size2;
            Vector2 size3 = indicatorsRects[3].sizeDelta;  // Get current size
            size3.x = width3;  // Set the new width
            indicatorsRects[3].sizeDelta = size3;
            Vector2 size4 = indicatorsRects[4].sizeDelta;  // Get current size
            size4.x = width4;  // Set the new width
            indicatorsRects[4].sizeDelta = size4;
            Vector2 size5 = indicatorsRects[5].sizeDelta;  // Get current size
            size5.x = width5;  // Set the new width
            indicatorsRects[5].sizeDelta = size5;
            
            yield return null;
        }

        elapsedTime = 0f;
        
        
        // Get the initial anchored positions of the images
        Vector2 startPosition1 = indicatorsTexts[0].anchoredPosition;
        Vector2 startPosition2 = indicatorsTexts[1].anchoredPosition;
        Vector2 startPosition3 = indicatorsTexts[2].anchoredPosition;

        // Calculate the target positions with the added Y offset
        Vector2 targetPosition1 = new Vector2(startPosition1.x, startPosition1.y + yOffset);
        Vector2 targetPosition2 = new Vector2(startPosition2.x, startPosition2.y + yOffset);
        Vector2 targetPosition3 = new Vector2(startPosition3.x, startPosition3.y + yOffset);

        // Animate the positions
        while (elapsedTime < durationText)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / durationText); // Normalize the time (0 to 1)

            // Interpolate each image's Y position, keeping the X fixed
            indicatorsTexts[0].anchoredPosition = Vector2.Lerp(startPosition1, targetPosition1, t);
            indicatorsTexts[1].anchoredPosition = Vector2.Lerp(startPosition2, targetPosition2, t);
            indicatorsTexts[2].anchoredPosition = Vector2.Lerp(startPosition3, targetPosition3, t);

            yield return null;  // Wait until the next frame
        }

        // Ensure final positions are exactly the target positions
        indicatorsTexts[0].anchoredPosition = targetPosition1;
        indicatorsTexts[1].anchoredPosition = targetPosition2;
        indicatorsTexts[2].anchoredPosition = targetPosition3;

        yield return new WaitForSeconds(4f);

        
        elapsedTime = 0f;
        while (elapsedTime < durationText)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / durationText); // Normalize the time (0 to 1)

            // Interpolate each image's Y position, keeping the X fixed
            indicatorsTexts[0].anchoredPosition = Vector2.Lerp(targetPosition1, startPosition1, t);
            indicatorsTexts[1].anchoredPosition = Vector2.Lerp(targetPosition2, startPosition2, t);
            indicatorsTexts[2].anchoredPosition = Vector2.Lerp(targetPosition3, startPosition3, t);

            yield return null;  // Wait until the next frame
        }
        
        // Ensure final positions are exactly the target positions
        indicatorsTexts[0].anchoredPosition = startPosition1;
        indicatorsTexts[1].anchoredPosition = startPosition2;
        indicatorsTexts[2].anchoredPosition = startPosition3;
        
        
        
        // Time in seconds for the alpha transition
        timeElapsed = 0f;
        
        while (timeElapsed < durationText)
        {
            timeElapsed += Time.deltaTime;
            float width  = Mathf.Lerp(widths[0], 0f, timeElapsed / durationText);
            float width1  = Mathf.Lerp(widths[1], 0f, timeElapsed / durationText);
            float width2  = Mathf.Lerp(widths[2], 0f, timeElapsed / durationText);
            float width3  = Mathf.Lerp(widths[3], 0f, timeElapsed / durationText);
            float width4  = Mathf.Lerp(widths[4], 0f, timeElapsed / durationText);
            float width5  = Mathf.Lerp(widths[5], 0f, timeElapsed / durationText);


            // Set the new width
            Vector2 size = indicatorsRects[0].sizeDelta;  // Get current size
            size.x = width;  // Set the new width
            indicatorsRects[0].sizeDelta = size;  // Apply the new size
            Vector2 size1 = indicatorsRects[1].sizeDelta;  // Get current size
            size1.x = width1;  // Set the new width
            indicatorsRects[1].sizeDelta = size1;
            Vector2 size2 = indicatorsRects[2].sizeDelta;  // Get current size
            size2.x = width2;  // Set the new width
            indicatorsRects[2].sizeDelta = size2;
            Vector2 size3 = indicatorsRects[3].sizeDelta;  // Get current size
            size3.x = width3;  // Set the new width
            indicatorsRects[3].sizeDelta = size3;
            Vector2 size4 = indicatorsRects[4].sizeDelta;  // Get current size
            size4.x = width4;  // Set the new width
            indicatorsRects[4].sizeDelta = size4;
            Vector2 size5 = indicatorsRects[5].sizeDelta;  // Get current size
            size5.x = width5;  // Set the new width
            indicatorsRects[5].sizeDelta = size5;
            
            yield return null;
        }
        
        
        inputAnimator.SetTrigger("BringUp");
        keysAnimator.SetTrigger("BringUp");
        
        yield return new WaitForSeconds(7f);
        
        weapon.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        pressKey.SetActive(true);
        var script = weapon.GetComponent<Weapon>();
        script.speed = 0f;
        while (!Input.GetKeyDown(KeyCode.J))
        {
            
            yield return null;
        }
        Destroy(weapon);
        pressKey.SetActive(false);
        playerSystem.animator.SetTrigger("Deflect");
        playerSystem.particleSystem.Play();
        playerSystem.parrySound.Play();
        playerSystem.inputState = true;
        yield return new WaitForSeconds(2f);
        
        scoreAnimator.SetTrigger("Score");
        weapons.SetActive(true);  
        

    }




    public IEnumerator ShowInstinct()
    {
        float timeElapsed = 0f;

        // Get the current color and set the starting alpha to 0
        Color instinctColor = instinctSprites[0].color;
        Color instinctColor2 = instinctSprites[1].color; 
        Color instinctColorx = instinctSprites[0].color;
        Color instinctColor2x = instinctSprites[1].color; 
        
        
        instinctColor.a = 0f;  // Start with alpha 0
        instinctColor2.a = 0f; 
        instinctColorx.a = 0f;  // Start with alpha 0
        instinctColor2x.a = 0f; 
        
        
        
        
        instinctSprites[0].color = instinctColor;
        instinctSprites[1].color = instinctColor2;
        instinctSprites[2].color = instinctColorx;
        instinctSprites[3].color = instinctColor2x;
        
        // Lerp the alpha from 0 to 1 over time
        while (timeElapsed < instinctDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / instinctDuration);
            float alpha2 = Mathf.Lerp(0f, 1f, timeElapsed / instinctDuration);
            float alphax = Mathf.Lerp(0f, 0.25f, timeElapsed / instinctDuration);
            float alpha2x = Mathf.Lerp(0f, 0.25f, timeElapsed / instinctDuration);
            
            
            instinctColor.a = alpha;
            instinctColor2.a = alpha2;
            instinctColorx.a = alphax;
            instinctColor2x.a = alpha2x;
          
            instinctSprites[0].color = instinctColor;
            instinctSprites[1].color = instinctColor2;
            instinctSprites[2].color = instinctColorx;
            instinctSprites[3].color = instinctColor2x;
            
            
            yield return null;
        }

        instinctColor.a = 1f;
        instinctColor2.a = 0.25f;
        instinctSprites[0].color = instinctColor;
        instinctSprites[1].color = instinctColor2;
        instinctSprites[2].color = instinctColor;
        instinctSprites[3].color = instinctColor2;

    }


    public IEnumerator HideInstinct()
    {
        float timeElapsed = 0f;

        // Get the current color and set the starting alpha to 0
        Color instinctColor = instinctSprites[0].color;
        Color instinctColor2 = instinctSprites[1].color; 
        Color instinctColorx = instinctSprites[0].color;
        Color instinctColor2x = instinctSprites[1].color; 
        
        
        instinctColor.a = 1f;  // Start with alpha 0
        instinctColor2.a = 1f; 
        instinctColorx.a = 0.25f;  // Start with alpha 0
        instinctColor2x.a = 0.25f; 
        
        
        
        
        instinctSprites[0].color = instinctColor;
        instinctSprites[1].color = instinctColor2;
        instinctSprites[2].color = instinctColorx;
        instinctSprites[3].color = instinctColor2x;
        
        // Lerp the alpha from 0 to 1 over time
        while (timeElapsed < instinctDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / instinctDuration);
            float alpha2 = Mathf.Lerp(1f, 0f, timeElapsed / instinctDuration);
            float alphax = Mathf.Lerp(0.25f, 0f, timeElapsed / instinctDuration);
            float alpha2x = Mathf.Lerp(0.25f, 0f, timeElapsed / instinctDuration);
            
            
            instinctColor.a = alpha;
            instinctColor2.a = alpha2;
            instinctColorx.a = alphax;
            instinctColor2x.a = alpha2x;
          
            instinctSprites[0].color = instinctColor;
            instinctSprites[1].color = instinctColor2;
            instinctSprites[2].color = instinctColorx;
            instinctSprites[3].color = instinctColor2x;
            
            
            yield return null;
        }

        instinctColor.a = 0f;
        instinctColor2.a = 0f;
        instinctSprites[0].color = instinctColor;
        instinctSprites[1].color = instinctColor2;
        instinctSprites[2].color = instinctColor;
        instinctSprites[3].color = instinctColor2;

    }
    private void PauseGameForTutorial()
    {
         // Pause the game
        pressKey.SetActive(true); // Show the tutorial text
        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator WaitForKeyPress()
    {
        // Wait until the specified key is pressed
        while (!Input.anyKeyDown)
        {
            playerSystem.inputState = true;
            yield return null;
        }

        ResumeGame();
    }

    private void ResumeGame()
    {
 
        pressKey.SetActive(false); // Hide the tutorial text
    }
        
        
        
        
        
        
        
        
        
        
        
        
        
    
}

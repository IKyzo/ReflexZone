using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;

public class TerminalInput : MonoBehaviour
{
    [SerializeField] private GameManager gameManager; // Reference to the GameManager script
    public TextMeshProUGUI inputText; // Reference to the TMP text field
    private string textContent = "";  // Stores user input
    private bool showCursor = true;   // Cursor visibility toggle
    private Coroutine cursorCoroutine;
    
    public bool allowTyping = false; // Flag to check if typing is in progress
    [SerializeField] private bool isCursorBlinking = false; // Flag to check if cursor blinking is enabled
    [SerializeField] private GameObject correctCheck;
    [SerializeField] private GameObject wrongCheck;
    
    private readonly int maxCharacters = 21; // Maximum allowed characters
    
    // Allow letters, numbers, and these characters: @ . _ -
    private readonly Regex validCharRegex = new Regex("^[a-zA-Z0-9@._-]$");
    private readonly Regex fullValidationRegex = new Regex("^[a-zA-Z0-9@._-]+$");


    void Start()
    {
        if (isCursorBlinking)
            cursorCoroutine = StartCoroutine(BlinkCursor());
    }

    void Update()
    {   
        if (allowTyping)
        {
            HandleTyping();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ValidateInput();
        }
    }

    void HandleTyping()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b' && textContent.Length > 0) // Handle backspace
            {
                textContent = textContent.Substring(0, textContent.Length - 1);
            }
            else if (validCharRegex.IsMatch(c.ToString()) && textContent.Length < maxCharacters)
            {
                textContent += c;
            }
        }
        UpdateText();
    }

    void ValidateInput()
    {
        if (textContent.Length >= 1 && textContent.Length <= maxCharacters && fullValidationRegex.IsMatch(textContent))
        {
            allowTyping = false; // Disable typing after validation
            correctCheck.SetActive(true);
            wrongCheck.SetActive(false);
            allowTyping = false; // Disable typing after validation
            gameManager.isGuest = true;
            gameManager.guestIndicator.SetActive(true); // Show guest indicator
            gameManager.playerName.text = textContent; // Set the player name in the GameManager
            gameManager.InitGame();

        }
        else
        {
            correctCheck.SetActive(false);
            wrongCheck.SetActive(true);
        }
    }

    void UpdateText()
    {
        string cursor = showCursor ? "▌" : "<alpha=#00>▌</alpha>";
        inputText.text = textContent + cursor;
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            showCursor = !showCursor;
            UpdateText();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnDestroy()
    {
        if (cursorCoroutine != null)
            StopCoroutine(cursorCoroutine);
    }
}

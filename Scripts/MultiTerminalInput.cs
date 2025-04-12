using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MultiTerminalInput : MonoBehaviour
{
    [System.Serializable]
    public class InputFieldData
    {
        public string fieldName; // Just for debugging clarity
        public TextMeshProUGUI inputText;
        public GameObject correctCheck;
        public GameObject wrongCheck;
        public bool isPassword;
        [HideInInspector] public string content = "";
    }

    [SerializeField] private FirebaseManager firebaseManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<InputFieldData> inputFields;
    private int currentInputIndex = 0;

    private bool allowTyping = true;
    private bool showCursor = true;
    private Coroutine cursorCoroutine;

    private readonly int maxCharacters = 21;
    private readonly Regex validCharRegex = new Regex("^[a-zA-Z0-9@._-]$");
    private readonly Regex fullValidationRegex = new Regex("^[a-zA-Z0-9@._-]+$");



    void Start()
    {
        cursorCoroutine = StartCoroutine(BlinkCursor());
        UpdateAllTextFields();
    }

    void Update()
    {
        if (!allowTyping) return;

        HandleArrowNavigation();

        foreach (char c in Input.inputString)
        {
            HandleCharacterInput(c);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ValidateAllFields();
        }
    }

    void HandleArrowNavigation()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentInputIndex = (currentInputIndex + 1) % inputFields.Count;
            UpdateAllTextFields();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentInputIndex = (currentInputIndex - 1 + inputFields.Count) % inputFields.Count;
            UpdateAllTextFields();
        }
    }

    void HandleCharacterInput(char c)
    {
        InputFieldData current = inputFields[currentInputIndex];

        if (c == '\b' && current.content.Length > 0)
        {
            current.content = current.content.Substring(0, current.content.Length - 1);
        }
        else if (validCharRegex.IsMatch(c.ToString()) && current.content.Length < maxCharacters)
        {
            current.content += c;
        }

        UpdateAllTextFields();
    }

void ValidateAllFields()
{
    bool allValid = true;
    string password = "";
    string repassword = "";

    for (int i = 0; i < inputFields.Count; i++)
    {
        var field = inputFields[i];
        string content = field.content;
        bool isValid = true;

        // Email check (assuming email is index 1 — adjust if needed)
        if (i == 1)
        {
            Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            isValid = emailRegex.IsMatch(content);
        }
        // General validation for username and password fields
        else
        {
            isValid = content.Length >= 1 &&
                      content.Length <= maxCharacters &&
                      fullValidationRegex.IsMatch(content);
        }

        if (field.correctCheck) field.correctCheck.SetActive(isValid);
        if (field.wrongCheck) field.wrongCheck.SetActive(!isValid);

        if (!isValid)
        {
            allValid = false;
        }

        // Store password and repassword
        if (i == 2) password = content;
        if (i == 3) repassword = content;
    }

    // Check if password and repassword match
    if (password != repassword)
    {
        allValid = false;

        var passField = inputFields[2];
        var repassField = inputFields[3];

        if (passField.correctCheck) passField.correctCheck.SetActive(false);
        if (passField.wrongCheck) passField.wrongCheck.SetActive(true);

        if (repassField.correctCheck) repassField.correctCheck.SetActive(false);
        if (repassField.wrongCheck) repassField.wrongCheck.SetActive(true);

        Debug.Log("❌ Passwords do not match.");
    }

    if (allValid)
    {
        allowTyping = false;
        Debug.Log("✅ All fields valid, proceeding...");
        gameManager.InitGame();
         string username = inputFields[0].content;
        string email = inputFields[1].content;
        password = inputFields[2].content;

        firebaseManager.CreateAccount(username, password, email);
        gameManager.isGuest = false;
        gameManager.guestIndicator.SetActive(false);
        gameManager.playerName.text = username;
        allowTyping = false; 

    }
    else
    {
        Debug.Log("⛔ One or more fields are invalid.");
    }
}




    bool AllFieldsValid()
    {
        foreach (var field in inputFields)
        {
            if (field.content.Length < 1 || field.content.Length > maxCharacters || !fullValidationRegex.IsMatch(field.content))
                return false;
        }
        return true;
    }

    void UpdateAllTextFields()
    {
        for (int i = 0; i < inputFields.Count; i++)
        {
            var field = inputFields[i];
            string textToDisplay = field.isPassword ? new string('*', field.content.Length) : field.content;
            string cursor = (i == currentInputIndex && showCursor) ? "▌" : "<alpha=#00>▌</alpha>";
            field.inputText.text = textToDisplay + cursor;
        }
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            showCursor = !showCursor;
            UpdateAllTextFields();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnDestroy()
    {
        if (cursorCoroutine != null)
            StopCoroutine(cursorCoroutine);
    }

    // Optional: expose method to get input values
    public string GetFieldValue(string fieldName)
    {
        var field = inputFields.Find(f => f.fieldName == fieldName);
        return field != null ? field.content : "";
    }
}

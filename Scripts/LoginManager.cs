using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;

public class LoginUIHandler : MonoBehaviour
{
    [System.Serializable]
    public class InputFieldData
    {
        public TextMeshProUGUI inputText;
        public GameObject correctCheck;
        public GameObject wrongCheck;
        public bool isPassword; 
        [HideInInspector] public string content = "";
    }

    [SerializeField] private List<InputFieldData> inputFields;
    [SerializeField] private FirebaseManager firebaseManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int maxCharacters = 21;

    private int selectedIndex = 0;
    private bool allowTyping = true;
    private readonly Regex validCharRegex = new Regex("^[a-zA-Z0-9@._-]$");
    private readonly Regex fullValidationRegex = new Regex("^[a-zA-Z0-9@._-]+$");
    private bool showCursor = true;
    private Coroutine cursorCoroutine;

    void Start()
    {
        cursorCoroutine = StartCoroutine(BlinkCursor());
        UpdateAllText();
    }

    void Update()
    {
        if (!allowTyping) return;

        // Navigation
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeField(-1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeField(1);

        // Typing
        foreach (char c in Input.inputString)
        {
            if (c == '\b' && inputFields[selectedIndex].content.Length > 0)
            {
                inputFields[selectedIndex].content = inputFields[selectedIndex].content.Substring(0, inputFields[selectedIndex].content.Length - 1);
            }
            else if (validCharRegex.IsMatch(c.ToString()) && inputFields[selectedIndex].content.Length < maxCharacters)
            {
                inputFields[selectedIndex].content += c;
            }
        }

        UpdateAllText();

        // Submit
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AttemptLogin();
        }
    }

    void ChangeField(int direction)
    {
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, inputFields.Count - 1);
        UpdateAllText();
    }

    void UpdateAllText()
    {
        for (int i = 0; i < inputFields.Count; i++)
        {
            string contentToShow = inputFields[i].isPassword
                ? new string('*', inputFields[i].content.Length)
                : inputFields[i].content;

            string cursor = (i == selectedIndex && showCursor) ? "▌" : "<alpha=#00>▌</alpha>";
            inputFields[i].inputText.text = contentToShow + cursor;
        }
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            showCursor = !showCursor;
            UpdateAllText();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void AttemptLogin()
    {
        string username = inputFields[0].content;
        string password = inputFields[1].content;

        bool validUsername = fullValidationRegex.IsMatch(username);
        bool validPassword = fullValidationRegex.IsMatch(password);

        // 🚫 Disable feedback until Firebase responds
        foreach (var field in inputFields)
        {
            field.correctCheck.SetActive(false);
            field.wrongCheck.SetActive(false);
        }

        if (!validUsername || !validPassword)
        {
            inputFields[0].wrongCheck.SetActive(!validUsername);
            inputFields[1].wrongCheck.SetActive(!validPassword);
            return;
        }

        allowTyping = false;

        firebaseManager.Login(username, password, success =>
        {
            allowTyping = true;

            inputFields[0].correctCheck.SetActive(success);
            inputFields[0].wrongCheck.SetActive(!success);

            inputFields[1].correctCheck.SetActive(success);
            inputFields[1].wrongCheck.SetActive(!success);

            if (success)
            {
                gameManager.isGuest = false;
                gameManager.playerName.text = username;
                gameManager.guestIndicator.SetActive(false);
                gameManager.InitGame();
                allowTyping = false; // Disable typing after successful login
            }
        });
    }

    void OnDestroy()
    {
        if (cursorCoroutine != null)
            StopCoroutine(cursorCoroutine);
    }
}

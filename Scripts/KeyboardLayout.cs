using UnityEngine;

public class KeyboardLayout : MonoBehaviour
{

    [SerializeField] private ThemeManager themeManager; // Reference to the ThemeManager script
    [SerializeField] private GameObject[] settingsPanels; // Array of layout labels
    
    // Panel 0 = Theme
    // Panel 1 = Authentification for Leaderboard 
    // Panel 1 = Keyboard Layout 
    // Game on. 
    [SerializeField] private GameObject[] themeItems; // Array of theme items
    [SerializeField] private GameObject[] layoutLabels;
    [SerializeField] private GameObject[] layoutKeyboardDisplay;

    [SerializeField] private int currentIndex = 0;
    [SerializeField] private int themeIndex = 0; // 0 = Dark, 1 = Light
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TerminalInput terminalInput;
    void Start()
    {
        UpdateLayout();
        UpdateTheme();

        settingsPanels[0].SetActive(true); // Show the first panel by default
    }


    void UpdateTheme()
    {
        themeItems[0].SetActive(themeIndex == 0);
        themeItems[1].SetActive(themeIndex == 1);
        if(themeIndex == 0){
            themeManager.SetupThemeDark(); // Set the initial theme to dark
            Debug.Log("Setting Dark");
        }
        else if(themeIndex == 1){
            themeManager.SetupThemeLight(); // Set the initial theme to light
            Debug.Log("Setting Light");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            LeftSelect();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            RightSelect();
        }
        else if (Input.GetKeyDown(KeyCode.Return)) 
        {
            EnterSelect(); // Call the EnterSelect method when Enter is pressed
        }
        else if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            BackspaceSelect(); // Call the BackspaceSelect method when Backspace is pressed
        }
    }


    public void EnterSelect()
    {
        if(settingsPanels[0].activeSelf){
            settingsPanels[0].SetActive(false); // Hide the first panel
            settingsPanels[1].SetActive(true); // Show the second panel
        }
        else if(settingsPanels[1].activeSelf){
            settingsPanels[1].SetActive(false); // Hide the second panel
            settingsPanels[2].SetActive(true); // Show the first panel
            terminalInput.allowTyping = true; // Allow typing in the terminal input
        }

    }
    public void BackspaceSelect()
    {
        if(settingsPanels[1].activeSelf){
            settingsPanels[1].SetActive(false); // Hide the second panel
            settingsPanels[0].SetActive(true); // Show the first panel
        }
        else if(settingsPanels[2].activeSelf){
            terminalInput.allowTyping = false; // Allow typing in the terminal input
            settingsPanels[2].SetActive(false); // Hide the first panel
            settingsPanels[1].SetActive(true); // Show the second panel

        }
    }

    public void LeftSelect()
    {   
        if( settingsPanels[0].activeSelf){
            themeIndex = ++themeIndex%2;
            UpdateTheme();
        }
        if(settingsPanels[1].activeSelf){
            currentIndex = (currentIndex - 1 + layoutLabels.Length) % layoutLabels.Length;
            UpdateLayout();
        }
        
    }
    public void RightSelect()
    {
        if( settingsPanels[0].activeSelf){
            themeIndex = ++themeIndex%2;
            UpdateTheme();
        }
        if(settingsPanels[1].activeSelf){
            currentIndex = (currentIndex + 1) % layoutLabels.Length;
            UpdateLayout();
        }
    }

    void UpdateLayout()
    {
        for (int i = 0; i < layoutLabels.Length; i++)
        {
            layoutLabels[i].SetActive(i == currentIndex);
            layoutKeyboardDisplay[i].SetActive(i == currentIndex);
        }
    }
}

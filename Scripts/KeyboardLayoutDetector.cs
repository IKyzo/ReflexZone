using UnityEngine;
using System.Globalization;
public class KeyboardLayoutDetector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //string layout = GetKeyboardLayout();
        Debug.Log("System current Lang : " + Application.systemLanguage.ToString());
    }
}

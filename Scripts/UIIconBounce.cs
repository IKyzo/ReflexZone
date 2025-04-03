using UnityEngine;

public class UIIconBounce : MonoBehaviour
{
    public RectTransform targetIcon;  // Assign your UI icon's RectTransform
    public float bounceHeight = 10f;  // How high it moves up
    public float bounceSpeed = 2f;    // Speed of movement

    private Vector2 startPosition;

    void Start()
    {
        if (targetIcon == null) targetIcon = GetComponent<RectTransform>();
        startPosition = targetIcon.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        targetIcon.anchoredPosition = startPosition + new Vector2(0, offset);
    }
}
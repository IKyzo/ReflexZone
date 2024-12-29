using UnityEngine;

public class CloudTranslation : MonoBehaviour
{
    // Rotation speed in degrees per second
    [SerializeField] private float translationSpeed = 50f;

    void FixedUpdate()
    {
        transform.Translate(translationSpeed * Time.deltaTime, 0f, 0f);
        if (transform.position.x < -20f)
        {
            this.gameObject.SetActive(false);
        }
    }
}

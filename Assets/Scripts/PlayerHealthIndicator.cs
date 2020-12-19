using UnityEngine;

public class PlayerHealthIndicator : MonoBehaviour
{
    public PlayerHealth health;
    TextMesh text;
    int displayedHealth = 0;

    void Start()
    {
        text = GetComponent<TextMesh>();
        text.text = $"{displayedHealth}";
    }

    void Update()
    {
        if (health.health != displayedHealth) {
            text.text = $"{health.health}";
            displayedHealth = health.health;
        }
    }
}

using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Assign in Inspector
    private float deltaTime = 0.0f;

    void Update()
    {
        // Exponential moving average for smooth FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        float ms = deltaTime * 1000.0f;
        if (fpsText != null)
            fpsText.text = $"FPS: {fps:F1}\nFrame: {ms:F1} ms";
    }
} 
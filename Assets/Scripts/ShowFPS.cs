using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour
{
    public Text fpsText; // Optional, if you want to display FPS with a UI Text element
    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate frame time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        // Calculate FPS
        float fps = 1.0f / deltaTime;

        // Optional: Display FPS on a UI Text element
        if (fpsText != null)
        {
            fpsText.text = string.Format("{0:0.} FPS", fps);
        }

        // Display FPS on screen
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
        string text = string.Format("{0:0.} FPS", fps);
        GUI.Label(rect, text, style);
    }
}


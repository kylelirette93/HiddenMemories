using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    void OnGUI()
    {
        int fps = (int)(1.0f / Time.deltaTime);
        GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + fps);
    }
}

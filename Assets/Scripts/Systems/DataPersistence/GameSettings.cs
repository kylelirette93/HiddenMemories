using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public float MouseSensitivity { get; set; } = 0.5f;


    private void Awake()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
        PlayerPrefs.Save();
    }
}

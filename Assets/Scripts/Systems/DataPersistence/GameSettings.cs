using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public float MouseSensitivity { get; set; } = 0.2f;


    private void Awake()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.2f);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
        PlayerPrefs.Save();
    }
}

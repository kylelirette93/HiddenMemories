using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;

    private void OnEnable()
    {
        sensitivitySlider.value = GameManager.Instance.gameSettings.MouseSensitivity;
        sensitivitySlider.onValueChanged.AddListener(value =>
        {
            GameManager.Instance.gameSettings.MouseSensitivity = value;
            GameManager.Instance.gameSettings.SaveSettings();
        });
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MotionBlurState : MonoBehaviour
{
    Toggle motionBlurToggle;
    public Volume globalVolume;
    public VolumeProfile profile;

    private void OnEnable()
    {
        globalVolume = FindFirstObjectByType<Volume>();
        motionBlurToggle = GetComponent<Toggle>();

        if (globalVolume != null && globalVolume.profile != null)
        {
            profile = Instantiate(globalVolume.profile);
            globalVolume.profile = profile;
        }
        motionBlurToggle.isOn = PlayerPrefs.GetInt("MotionBlur", 1) == 1;
    }

    public void ApplyMotionBlur(bool active)
    {
        if (globalVolume && globalVolume.profile &&
            globalVolume.profile.TryGet(out MotionBlur motionBlur))
        {
            motionBlur.active = motionBlurToggle.isOn;
            PlayerPrefs.SetInt("MotionBlur", motionBlurToggle.isOn ? 1 : 0);
        }
    }
}

using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    private Quaternion lockedRotation;

    void Start()
    {
        // Store the initial rotation
        lockedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // Reset rotation every frame
        transform.rotation = lockedRotation;
    }
}
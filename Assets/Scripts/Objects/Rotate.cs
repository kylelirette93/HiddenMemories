using UnityEngine;

public class Rotate : MonoBehaviour
{
    float rotationSpeed = 50f;

    public void Update()
    {
        Vector3 rotationVector = new Vector3(rotationSpeed, rotationSpeed, rotationSpeed);
        transform.Rotate(rotationVector * Time.deltaTime, Space.Self);
    }
}

using UnityEngine;

public class Rotate : MonoBehaviour
{
    float rotationSpeed = 50f;

    public void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}

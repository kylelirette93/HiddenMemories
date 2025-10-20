using UnityEngine;

public class VomitCollisionHandler : MonoBehaviour
{
    GameObject splatPrefab;
    int numOfCollisions;
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            numOfCollisions++;
            Camera playerCamera = other.GetComponentInChildren<Camera>();
        }
    }

    private void OnParticleSystemStopped()
    {
        numOfCollisions = 0;
    }
}

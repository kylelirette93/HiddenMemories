using UnityEngine;

/// <summary>
/// Gizmo for drawing a radius, useful for spawn points.
/// </summary>
public class Gizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 2);
    }
}

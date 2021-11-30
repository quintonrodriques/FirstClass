using UnityEngine;

public class Obsticle : MonoBehaviour
{
    public float radius = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
using UnityEngine;

public class Obsticle : MonoBehaviour
{
    public float radius = 1.0f;

    public void Start()
    {
        BoidManager.AddObsticle(this);
    }

    public void OnDestroy()
    {
        BoidManager.RemoveObsticle(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
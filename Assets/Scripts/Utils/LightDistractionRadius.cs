using UnityEngine;

public class LightDistractionRadius : MonoBehaviour
{
    public float distractionRad = 20f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, distractionRad);
    }
}

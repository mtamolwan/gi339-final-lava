using UnityEngine;

public class DangerObject : MonoBehaviour
{
    public float knockbackForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                
                Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;
                
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}

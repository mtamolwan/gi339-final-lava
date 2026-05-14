using UnityEngine;
using System.Collections;

public class FallingTrap : MonoBehaviour
{
    public float fallDelay = 0.5f; 
    private Rigidbody rb;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // เปลี่ยนจาก OnCollisionEnter เป็น OnTriggerEnter
    // และเปลี่ยน Collision เป็น Collider
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("มีคนมาเหยียบกล่อง! ชื่อ: " + other.gameObject.name + " ป้ายTagคือ: " + other.gameObject.tag);

        if (other.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(DropPlatform());
        }
    }

    IEnumerator DropPlatform()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallDelay);
        rb.isKinematic = false; 
        rb.linearVelocity = new Vector3(0, -20f, 0);
        Destroy(gameObject, 1f); 
    }
}
using UnityEngine;

public class DangerSpawner : MonoBehaviour
{
    public GameObject dangerPrefab;
    public float spawnRate = 0.5f;   
    public float explosionForce = 500f; 

    void Start()
    {
        InvokeRepeating("SpawnObject", 0f, spawnRate);
    }

    void SpawnObject()
    {
        
        GameObject obj = Instantiate(dangerPrefab, transform.position, Quaternion.identity);

        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
            rb.AddForce(randomDir * explosionForce);
        }

       
        Destroy(obj, 3f);
    }
}

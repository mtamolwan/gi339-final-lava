using UnityEngine;

public class Spawn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject obstaclePrefab; // Drag your prefab here in the Inspector
    public float spawnInterval = 2.0f; // Seconds between spawns
    private float startDelay = 2.0f;
    public Vector3 spawnRange = new Vector3(10, 0, 1);
    void Start()
    {
        // Start the spawning process
        InvokeRepeating("SpawnObject", startDelay, spawnInterval);
    }
    // Update is called once per frame
    void SpawnObject()
    {
        Vector3 randomPos = new Vector3(
           Random.Range(-spawnRange.x, spawnRange.x),
           Random.Range(-spawnRange.y, spawnRange.y),
           Random.Range(-spawnRange.z, spawnRange.z)
       ) + transform.position;
        Instantiate(obstaclePrefab, randomPos, Quaternion.identity);
    }
}

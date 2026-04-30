using UnityEngine;

public class WaterKillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าคนที่ตกลงมาโดนน้ำคือผู้เล่นใช่ไหม
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over");
            
        
            FindFirstObjectByType<GameOverManager>().SetupGameOver();
        }
    }
}
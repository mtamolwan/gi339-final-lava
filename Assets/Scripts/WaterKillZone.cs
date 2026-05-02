using UnityEngine;

public class WaterKillZone : MonoBehaviour
{
    [Header("ลากก้อน BGM มาใส่ช่องนี้")]
    public GameObject bgmObject;

    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าคนที่ตกลงมาโดนน้ำคือผู้เล่นใช่ไหม
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over");
            
            // 🛑 สั่งปิดออบเจกต์ BGM ทิ้ง เสียงทุกอย่างที่แปะอยู่จะดับสนิท!
            if (bgmObject != null)
            {
                bgmObject.SetActive(false);
            }
            
            FindFirstObjectByType<GameOverManager>().SetupGameOver();
        }
    }
}
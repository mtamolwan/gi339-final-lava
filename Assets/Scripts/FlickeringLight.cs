using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    private Light myLight;
    
    [Header("ตั้งค่าความเร็วในการกระพริบ")]
    public float minWaitTime = 0.05f; // กระพริบเร็วสุด (เสี้ยววินาที)
    public float maxWaitTime = 0.3f;  // กระพริบช้าสุด

    void Start()
    {
        // ดึงคอมโพเนนต์หลอดไฟมาเตรียมไว้
        myLight = GetComponent<Light>();
        
        // เริ่มสั่งงานกระพริบทันที
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        // ทำงานวนซ้ำไปเรื่อยๆ ไม่มีวันหยุด
        while (true)
        {
            // สลับสถานะหลอดไฟ (ถ้าเปิดอยู่ให้ปิด ถ้าปิดอยู่ให้เปิด)
            myLight.enabled = !myLight.enabled; 
            
            // สุ่มเวลารอ ก่อนที่จะสลับไฟอีกรอบ
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }
    // เพิ่มส่วนนี้เข้าไปในสคริปต์ FlickeringLight นะครับ
    public void StopFlickering()
    {
        // หยุดการกระพริบทั้งหมด
        StopAllCoroutines(); 
        
        // ดึงคอมโพเนนต์ Light มาสั่งให้เปิดค้างไว้
        Light myLight = GetComponent<Light>();
        if (myLight != null)
        {
            myLight.enabled = true;
        }
    }
}
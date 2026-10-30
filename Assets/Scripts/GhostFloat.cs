using UnityEngine;

public class GhostFloat : MonoBehaviour
{
    [Header("การลอยไปมาตามแกน X")]
    public float distance = 3f;   // ระยะที่ลอยไปจากจุดกลางแต่ละข้าง
    public float speed = 1.5f;    // ความเร็ว

    [Header("ลอยขึ้นลงเบาๆ (ใส่ 0 ถ้าไม่เอา)")]
    public float bobHeight = 0.2f;
    public float bobSpeed = 2f;

    private Vector3 startPos;
    private float lastX;

    void Start()
    {
        startPos = transform.position;
        lastX = startPos.x;
    }

    void Update()
    {
        float newX = startPos.x + Mathf.Sin(Time.time * speed) * distance;
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(newX, newY, startPos.z);

        // หันหน้าตามทิศที่ลอยไป
        float dir = newX - lastX;
        if (Mathf.Abs(dir) > 0.0001f)
        {
            float yAngle = dir > 0 ? 90f : -90f;
            transform.rotation = Quaternion.Euler(0, yAngle, 0);
        }
        lastX = newX;
    }
}
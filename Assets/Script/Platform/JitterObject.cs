using UnityEngine;
using System.Collections;

public class JitterObject : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("ระยะเวลาพักระหว่างการกระตุกแต่ละรอบ (วินาที)")]
    public float cooldownTime = 5.0f;

    [Tooltip("ระยะเวลาที่วัตถุจะทำการกระตุก (วินาที)")]
    public float jitterDuration = 0.5f;

    [Header("Jitter Settings")]
    [Tooltip("ความเร็วในการสั่น (ยิ่งเยอะยิ่งดูถี่ยิบเหมือนไฟฟ้าช็อต)")]
    public float jitterSpeed = 50.0f;

    [Tooltip("ความแรง/ระยะทางการสั่น (ยิ่งเยอะยิ่งสั่นกว้าง)")]
    public float jitterAmount = 0.1f;

    private Vector3 originalPosition;
    private float timer;

    void Start()
    {
        // บันทึกตำแหน่งเริ่มต้นไว้ เพื่อให้กระตุกเสร็จแล้วกลับมาที่เดิม
        originalPosition = transform.localPosition;
        timer = cooldownTime;
    }

    void Update()
    {
        // นับถอยหลัง Cooldown
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            StartCoroutine(DoJitter());
            timer = cooldownTime; // รีเซ็ตเวลา Cooldown
        }
    }

    IEnumerator DoJitter()
    {
        float elapsed = 0.0f;

        while (elapsed < jitterDuration)
        {
            elapsed += Time.deltaTime;

            // คำนวณการขยับขึ้นลงแบบรวดเร็ว (ใช้ Sine Wave ความถี่สูง)
            // หรือใช้ Random.Range หากต้องการความไม่เป็นระเบียบ
            float offset = Mathf.Sin(Time.time * jitterSpeed) * jitterAmount;

            // ใช้ localPosition เพื่อไม่ให้กระทบกับการเคลื่อนที่หลักของ Object
            transform.localPosition = originalPosition + new Vector3(0, offset, 0);

            yield return null; // รอเฟรมถัดไป
        }

        // เมื่อจบการกระตุก ให้กลับไปตำแหน่งเดิมเป๊ะๆ
        transform.localPosition = originalPosition;
    }
}
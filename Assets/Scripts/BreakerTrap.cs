using UnityEngine;
using System.Collections;

public class BreakerTrap : MonoBehaviour
{
    [Header("ลากสคริปต์ตู้ไฟมาใส่ตรงนี้")]
    public BreakerSkillCheck breakerBox; 
    
    [Header("ความแรงตอนกระเด็นตกน้ำ")]
    public float knockbackStrength = 15f; 

    void OnTriggerEnter(Collider other)
    {
        // ถ้าคนที่เดินชนคือผู้เล่น และ ตู้ไฟยังไม่ถูกซ่อม!
        if (other.CompareTag("Player") && breakerBox != null && !breakerBox.isFixed)
        {
            Debug.Log("เดินผ่านตู้ไฟที่ช็อตอยู่ โดนระเบิดใส่!");
            breakerBox.TriggerExplosion();

            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                // สั่งให้กระเด็น
                StartCoroutine(ApplyKnockback(cc));
            }
        }
    }

    private IEnumerator ApplyKnockback(CharacterController playerController)
    {
        // หาทิศทางกระเด็น (ผลักออกจากตู้ไฟ)
        Vector3 pushDirection = playerController.transform.position - breakerBox.transform.position;
        pushDirection.y = 0; 
        pushDirection = pushDirection.normalized; 
        pushDirection.y = 0.5f; // แถมแรงยกตัวลอยนิดๆ

        float duration = 0.2f; 
        float elapsed = 0f;
        float pushSpeed = knockbackStrength * 4f; 

        while (elapsed < duration)
        {
            playerController.Move(pushDirection * pushSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null; 
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // จำเป็นสำหรับปิดการบังคับของ Starter Assets

public class PictureJumpscare : MonoBehaviour
{
    [Header("ใส่กล้องจั้มสะแกร์ (ที่ถูกปิดไว้)")]
    public GameObject jumpscareCamera; 
    
    [Header("ตั้งค่า")]
    public AudioClip scareSound;
    public float holdTime = 3.0f;

    private AudioSource audioSource;
    private bool isRunning = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // เช็คว่าคนที่ชนคือ PlayerArmature ใช่ไหม
        if (other.CompareTag("Player") && !isRunning)
        {
            StartCoroutine(PerformJumpscare(other.gameObject));
        }
    }

    IEnumerator PerformJumpscare(GameObject player)
    {
        isRunning = true;

        // 1. หยุดตัวละคร: ปิดระบบ PlayerInput (ตัวละครจะหยุดเดินและหยุดหมุนกล้องทันที)
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.enabled = false;

        // 2. กระชากหน้า: สั่งเปิด JumpscareCamera ที่เราจัดมุมไว้
        if (jumpscareCamera != null) jumpscareCamera.SetActive(true);

        // 3. กรี๊ด!: เล่นเสียง
        if (audioSource != null && scareSound != null) audioSource.PlayOneShot(scareSound);

        // 4. ล็อคคอ: รอเวลา 3 วินาที
        yield return new WaitForSeconds(holdTime);

        // 5. ปล่อยตัว: ปิด JumpscareCamera (มุมมองจะเด้งกลับไปที่กล้องหลักอัตโนมัติ)
        if (jumpscareCamera != null) jumpscareCamera.SetActive(false);

        // 6. ให้เดินต่อ: เปิดระบบ PlayerInput กลับมา
        if (playerInput != null) playerInput.enabled = true;

        // ทำลายกล่องล่องหนทิ้ง จะได้ไม่ตกใจซ้ำ
        Destroy(gameObject);
    }
}
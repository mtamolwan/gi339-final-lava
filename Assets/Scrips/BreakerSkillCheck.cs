using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem; 

public class BreakerSkillCheck : MonoBehaviour
{
    [Header("🎥 กล้องตอนเล่นมินิเกม")]
    public GameObject minigameCamera; 

    [Header("ตั้งค่าการกระเด็น")]
    public float knockbackStrength = 15f; 

    [Header("💥 แสงสีเสียงตอนระเบิด")]
    public GameObject explosionVFX; // ใส่ Prefab เอฟเฟคระเบิด
    public AudioClip explosionSound; // ใส่ไฟล์เสียงระเบิด
    private AudioSource audioSource; // ตัวเล่นเสียง

    [Header("เอฟเฟคไกด์นำทาง")]
    public GameObject sparkEffect; 

    public bool isFixed = false; 

    [Header("ใส่สคริปต์หลอดไฟทั้งหมดที่ต้องการซ่อม")]
    public FlickeringLight[] lightsToFix;

    [Header("จัดส่วนประกอบ UI (ภายใต้ World Space Canvas)")]
    public GameObject minigameUI;      
    public RectTransform containerRect;   
    public RectTransform targetZoneRect;  
    public RectTransform sliderHandRect; 

    [Header("ตั้งค่า")]
    public float sliderSpeed = 100f; 
    private int requiredSuccesses = 5; 

    private int currentSuccesses = 0;
    private bool playerInZone = false;
    private bool isGameActive = false;

    private CharacterController playerController;
    private float containerWidth;
    private float targetZoneWidth;
    private float moveDirection = 1f; 
    private PlayerInput playerInput;

    void Start()
    {
        if (minigameUI != null) minigameUI.SetActive(false);
        if (minigameCamera != null) minigameCamera.SetActive(false);
        
        containerWidth = containerRect.rect.width;
        targetZoneWidth = targetZoneRect.rect.width;
        RandomizeTargetZone();

        // สร้างตัวเล่นเสียงให้อัตโนมัติ (จะได้ไม่ต้องไปกด Add Component เอง)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (playerInZone && !isGameActive && !isFixed && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartMinigame();
            return; 
        }

        if (isGameActive)
        {
            float currentX = sliderHandRect.anchoredPosition.x;
            float newX = currentX + (moveDirection * sliderSpeed * Time.deltaTime);
            float halfWidth = containerWidth / 2f;

            if (newX >= halfWidth) { newX = halfWidth; moveDirection = -1f; }
            else if (newX <= -halfWidth) { newX = -halfWidth; moveDirection = 1f; }

            sliderHandRect.anchoredPosition = new Vector2(newX, sliderHandRect.anchoredPosition.y);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                CheckSkillCheck(newX);
            }
        }
    }

    void StartMinigame()
    {
        isGameActive = true;
        minigameUI.SetActive(true); 
        currentSuccesses = 0;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null) playerInput.enabled = false;
        }

        if (minigameCamera != null) minigameCamera.SetActive(true);

        RandomizeTargetZone(); 
    }

    void CheckSkillCheck(float handX)
    {
        float targetX = targetZoneRect.anchoredPosition.x;
        float halfTarget = targetZoneWidth / 2f;
        float targetMin = targetX - halfTarget;
        float targetMax = targetX + halfTarget;

        if (handX >= targetMin && handX <= targetMax)
        {
            currentSuccesses++;
            RandomizeTargetZone();
            if (currentSuccesses >= requiredSuccesses) CompleteRepair();
        }
        else
        {
            // --- พลาด! ---
            if (playerInput != null) playerInput.enabled = true;
            isGameActive = false;
            minigameUI.SetActive(false);
            if (minigameCamera != null) minigameCamera.SetActive(false);
            currentSuccesses = 0;

            // ⚠️ สั่งระเบิดตู้ม!
            TriggerExplosion(); 
            StartCoroutine(ApplyKnockback());
        }
    }

    // ⚠️ ฟังก์ชันระเบิด (ให้กำแพงกับดักมายืมใช้ได้ด้วย)
    public void TriggerExplosion()
    {
        Debug.Log("ระเบิดตู้ม!");
        
        // 1. เล่นเสียง
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // 2. เสกเอฟเฟคระเบิดตรงหน้าตู้ไฟ
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, transform.rotation);
        }
    }

    private IEnumerator ApplyKnockback()
    {
        if (playerController == null) yield break;

        Vector3 pushDirection = playerController.transform.position - transform.position;
        pushDirection.y = 0; 
        pushDirection = pushDirection.normalized; 
        pushDirection.y = 0.5f; 

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

    void CompleteRepair()
    {
        isFixed = true;
        isGameActive = false;
        minigameUI.SetActive(false); 
        if (minigameCamera != null) minigameCamera.SetActive(false);
        if (playerInput != null) playerInput.enabled = true; 
        if (sparkEffect != null) sparkEffect.SetActive(false); 

        foreach (FlickeringLight light in lightsToFix)
        {
            if (light != null) light.StopFlickering();
        }
    }

    void RandomizeTargetZone()
    {
        float maxPos = (containerWidth / 2f) - (targetZoneWidth / 2f);
        float randomX = Random.Range(-maxPos, maxPos);
        targetZoneRect.anchoredPosition = new Vector2(randomX, targetZoneRect.anchoredPosition.y);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFixed)
        {
            playerInZone = true;
            playerController = other.GetComponent<CharacterController>(); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (isGameActive)
            {
                isGameActive = false;
                minigameUI.SetActive(false);
                if (playerInput != null) playerInput.enabled = true; 
                if (minigameCamera != null) minigameCamera.SetActive(false);
            }
        }
    }
}
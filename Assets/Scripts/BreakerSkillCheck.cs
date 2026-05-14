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

    [Header("💥 แสงสีเสียง")]
    public GameObject explosionVFX; 
    public AudioClip explosionSound; 
    public AudioClip successSound; // เสียงตอนซ่อมเสร็จทั้งหมด
    
    [Header("✨ เพิ่มใหม่: เสียงตอนกดถูกจังหวะ!")]
    public AudioClip hitTargetSound; 

    private AudioSource audioSource; 

    [Header("เอฟเฟคไกด์นำทาง")]
    public GameObject sparkEffect; 
    
    [Header("✨ เพิ่มใหม่: ลาก Canvas ปุ่ม E มาใส่ตรงนี้")]
    public GameObject interactionPrompt; // ปุ่ม E หน้าตู้

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
        if (interactionPrompt != null) interactionPrompt.SetActive(false); // ซ่อนปุ่ม E ไว้ก่อนตอนเริ่มเกม
        
        containerWidth = containerRect.rect.width;
        targetZoneWidth = targetZoneRect.rect.width;
        RandomizeTargetZone();

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
        
        // ✨ ซ่อนปุ่ม E หน้าตู้ทันทีที่เริ่มเล่นมินิเกม
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        
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
            
            // ✨ เล่นเสียง "ถูกต้อง!" ตอนกดโดนเป้า
            if (hitTargetSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitTargetSound);
            }

            RandomizeTargetZone();
            if (currentSuccesses >= requiredSuccesses) CompleteRepair();
        }
        else
        {
            if (playerInput != null) playerInput.enabled = true;
            isGameActive = false;
            minigameUI.SetActive(false);
            if (minigameCamera != null) minigameCamera.SetActive(false);
            currentSuccesses = 0;

            TriggerExplosion(); 
            StartCoroutine(ApplyKnockback());
        }
    }

    public void TriggerExplosion()
    {
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

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

        if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        foreach (FlickeringLight light in lightsToFix)
        {
            if (light != null) light.StopFlickering();
        }
        
        Debug.Log("ไฟซ่อมเสร็จแล้ว!");
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
            
            // ✨ โชว์ปุ่ม E เวลาเดินเข้ามาใกล้
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            
            // ✨ ซ่อนปุ่ม E เวลาเดินหนีไป (หรือเวลากระเด็น)
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
            
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
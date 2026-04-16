using UnityEngine;
using System.Collections;

public class TimedPlatform : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ระยะเวลา (วินาที) หลังจากเหยียบแล้วแพลตฟอร์มจะหายไป")]
    public float fallDelay = 2.0f;

    [Tooltip("ระยะเวลาที่จะรอให้แพลตฟอร์มทำลายตัวเองทิ้ง (หลังจากหายไปแล้ว)")]
    public float destroyDelay = 1.0f;

    private bool isStepped = false;
    private MeshRenderer meshRenderer;
    private Collider platformCollider;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบว่าสิ่งที่มาเหยียบคือ Player และยังไม่มีการเริ่มนับถอยหลัง
        if (collision.gameObject.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (isStepped)
        {
            return;
        }

        isStepped = true;
        StartCoroutine(StartDisappearing());
    }

    IEnumerator StartDisappearing()
    {
        // รอตามเวลาที่ตั้งไว้
        yield return new WaitForSeconds(fallDelay);

        // ทำให้แพลตฟอร์มหายไป (ปิดการมองเห็นและปิดการชน)
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (platformCollider != null) platformCollider.enabled = false;

        // รออีกสักพักก่อนจะลบ Object ทิ้ง (หรือจะลบเลยก็ได้)
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}

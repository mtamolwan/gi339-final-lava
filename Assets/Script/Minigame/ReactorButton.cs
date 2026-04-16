using UnityEngine;

public class ReactorButton : MonoBehaviour
{
    public int buttonID; // ตั้งค่าใน Inspector: ปุ่ม 1 = 0, ปุ่ม 2 = 1, ปุ่ม 3 = 2
    public ReactorPuzzleManager manager;

    private MeshRenderer meshRenderer;
    private bool isPressed = false;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบว่าคนเหยียบคือ Player และยังไม่ได้ถูกกด
        if (collision.gameObject.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (isPressed)
        {
            return;
        }

        isPressed = true;
        if (meshRenderer != null)
        {
            meshRenderer.material.color = Color.green; // เปลี่ยนสีเมื่อเหยียบถูก
        }

        if (manager != null)
        {
            manager.ButtonStepped(buttonID);
        }
    }

    // ฟังก์ชันสำหรับรีเซ็ตปุ่มเมื่อเหยียบผิดลำดับ
    public void ResetButton()
    {
        isPressed = false;
        if (meshRenderer != null)
        {
            meshRenderer.material.color = Color.white; // กลับเป็นสีเดิม
        }
    }
}

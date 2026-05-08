using UnityEngine;

public class ReactorPuzzleManager : MonoBehaviour
{
    public int totalButtons = 3;
    private int currentStep = 0;
    public bool isPuzzleComplete = false;

    public void ButtonStepped(int id)
    {
        if (isPuzzleComplete) return;

        if (id == currentStep)
        {
            currentStep++;
            Debug.Log("กดถูกลำดับ! ขั้นถัดไป: " + currentStep);

            QuestUIManager.Instance.UpdateButtons(currentStep);

            if (currentStep >= totalButtons)
            {
                isPuzzleComplete = true;
                Debug.Log("Puzzle เสร็จสิ้น! ไปซ่อมเครื่องปฏิกรณ์ได้");
            }
        }
        else
        {
            Debug.Log("ลำดับผิด! รีเซ็ตใหม่ทั้งหมด");
            ResetPuzzle();
        }
    }

    void ResetPuzzle()
    {
        currentStep = 0;
        QuestUIManager.Instance.UpdateButtons(currentStep);

        // สั่งให้ทุกปุ่มในฉากรีเซ็ตตัวเอง
        ReactorButton[] buttons = FindObjectsByType<ReactorButton>(FindObjectsSortMode.None);
        foreach (ReactorButton b in buttons)
        {
            b.ResetButton();
        }
    }
}

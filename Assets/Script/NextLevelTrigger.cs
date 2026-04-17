using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นต้องใช้เพื่อเปลี่ยน Scene

public class NextLevelTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ใส่ชื่อ Scene ที่ต้องการไป (ถ้าว่างไว้จะไป Scene ถัดไปตามลำดับ Build Settings)")]
    public string sceneName = "";

    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบว่าผู้เล่นเป็นคนเหยียบ
        if (collision.gameObject.CompareTag("Player"))
        {
            GoToNextScene();
        }
    }

    // หากต้องการใช้เป็น Trigger (เดินทะลุผ่านแล้วเปลี่ยนฉาก) ให้ใช้ฟังก์ชันนี้แทน
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GoToNextScene();
        }
    }
    */

    void GoToNextScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // ถ้าใส่ชื่อ Scene ไว้ใน Inspector ให้ไปตามชื่อนั้น
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // ถ้าไม่ได้ใส่ชื่อไว้ ให้ไป Scene ลำดับถัดไปใน Build Settings (+1 จากปัจจุบัน)
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // ตรวจสอบก่อนว่ามี Scene ถัดไปไหม เพื่อป้องกัน Error
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("ไม่มี Scene ถัดไปใน Build Settings แล้วครับ!");
            }
        }
    }
}
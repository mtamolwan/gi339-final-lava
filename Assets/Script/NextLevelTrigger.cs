using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นต้องใช้เพื่อเปลี่ยน Scene

public class NextLevelTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ใส่ชื่อ Scene ที่ต้องการไป (ถ้าว่างไว้จะไป Scene ถัดไปตามลำดับ Build Settings)")]
    public string sceneName = "";

    private bool isLoading = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = GoToNextScene();
    }

    bool GoToNextScene()
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogWarning($"Scene '{sceneName}' ยังโหลดไม่ได้ ตรวจชื่อ Scene และ Build Settings อีกครั้ง");
                return false;
            }

            SceneManager.LoadScene(sceneName);
            return true;
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            return true;
        }

        Debug.LogWarning("ไม่มี Scene ถัดไปใน Build Settings แล้วครับ!");
        return false;
    }
}

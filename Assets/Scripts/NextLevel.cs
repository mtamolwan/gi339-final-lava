using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [Header("ชื่อไฟล์ด่านต่อไปที่จะให้ไป")]
    public string nextLevelName = "lvl_2_toilet";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var manager = FindFirstObjectByType<GhostFormManager>();
        if (manager != null) manager.RestoreHuman();

        Debug.Log("เหยียบเส้นชัยแล้ว! กำลังโหลดด่านต่อไป...");
        SceneManager.LoadScene(nextLevelName);
    }
}
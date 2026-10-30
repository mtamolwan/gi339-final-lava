using UnityEngine;

public class GhostKill : MonoBehaviour
{
    [Header("ลากก้อน BGM มาใส่ช่องนี้")]
    public GameObject bgmObject;

    private bool hasKilled = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasKilled) return;

        if (other.CompareTag("Player"))
        {
            hasKilled = true;
            Debug.Log("Game Over (โดนผี)");

            if (bgmObject != null)
            {
                bgmObject.SetActive(false);
            }

            FindFirstObjectByType<GameOverManager>().SetupGameOver();
        }
    }
}
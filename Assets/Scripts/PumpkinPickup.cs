using UnityEngine;

public class PumpkinPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var manager = FindFirstObjectByType<GhostFormManager>();
        if (manager != null) manager.BecomeGhost();

        gameObject.SetActive(false);
    }
}
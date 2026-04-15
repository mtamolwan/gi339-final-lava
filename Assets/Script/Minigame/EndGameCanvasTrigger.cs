using StarterAssets;
using UnityEngine;

public class EndGameCanvasTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject endGameCanvas;
    public AuditionMinigame requiredMinigame;

    [Header("Settings")]
    public bool requireMinigameCompletion = true;
    public bool triggerOnce = true;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private bool hasActivated = false;

    void Awake()
    {
        CachePlayerReferences();

        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (triggerOnce && hasActivated)
        {
            return;
        }

        if (!CanShowEndGameCanvas())
        {
            return;
        }

        ShowEndGameCanvas();
    }

    private bool CanShowEndGameCanvas()
    {
        if (!requireMinigameCompletion)
        {
            return true;
        }

        AssignMinigameIfMissing();
        return requiredMinigame != null && requiredMinigame.IsCompleted;
    }

    private void ShowEndGameCanvas()
    {
        if (endGameCanvas == null)
        {
            Debug.LogWarning("End game canvas is not assigned.");
            return;
        }

        CachePlayerReferences();

        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = false;
        }

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.cursorLocked = false;
            starterAssetsInputs.cursorInputForLook = false;
            starterAssetsInputs.MoveInput(Vector2.zero);
            starterAssetsInputs.LookInput(Vector2.zero);
            starterAssetsInputs.JumpInput(false);
            starterAssetsInputs.SprintInput(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        endGameCanvas.SetActive(true);
        hasActivated = true;
    }

    private void CachePlayerReferences()
    {
        if (thirdPersonController == null)
        {
            thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
        }

        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = FindFirstObjectByType<StarterAssetsInputs>();
        }
    }

    private void AssignMinigameIfMissing()
    {
        if (requiredMinigame != null)
        {
            return;
        }

        AuditionMinigame[] minigames = Resources.FindObjectsOfTypeAll<AuditionMinigame>();
        foreach (AuditionMinigame minigame in minigames)
        {
            if (minigame != null && minigame.gameObject.scene.IsValid())
            {
                requiredMinigame = minigame;
                return;
            }
        }
    }
}

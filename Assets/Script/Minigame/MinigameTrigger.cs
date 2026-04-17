using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MinigameTrigger : MonoBehaviour
{
    public GameObject minigameCanvas;
    public ReactorPuzzleManager puzzleManager;
    private bool canInteract = false;

    void Awake()
    {
        AssignMinigameCanvasIfMissing();
    }

    void Update()
    {
        if (canInteract && puzzleManager != null && puzzleManager.isPuzzleComplete && WasInteractPressedThisFrame())
        {
            StartMinigame();
        }
    }

    void StartMinigame()
    {
        AssignMinigameCanvasIfMissing();
        if (minigameCanvas == null)
        {
            Debug.LogWarning("Minigame canvas is not assigned.");
            return;
        }

        if (minigameCanvas.activeSelf)
        {
            return;
        }

        minigameCanvas.SetActive(true);

        AuditionMinigame auditionMinigame = minigameCanvas.GetComponent<AuditionMinigame>();
        if (auditionMinigame != null)
        {
            auditionMinigame.SetupGame();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = false;
    }

    private void AssignMinigameCanvasIfMissing()
    {
        if (minigameCanvas != null)
        {
            return;
        }

        AuditionMinigame[] minigames = Resources.FindObjectsOfTypeAll<AuditionMinigame>();
        foreach (AuditionMinigame minigame in minigames)
        {
            if (minigame != null && minigame.gameObject.scene.IsValid())
            {
                minigameCanvas = minigame.gameObject;
                return;
            }
        }
    }

    private bool WasInteractPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }
}

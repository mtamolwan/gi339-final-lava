using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;

    private StarterAssetsInputs starterAssetsInputs;

    void Awake()
    {
        starterAssetsInputs = FindFirstObjectByType<StarterAssetsInputs>();
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        SetGameplayCursorState();
    }

    public void SetupGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        SetGameOverCursorState();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SetGameplayCursorState();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetGameplayCursorState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateStarterAssetsInputState(true, true);
    }

    private void SetGameOverCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateStarterAssetsInputState(false, false);
    }

    private void UpdateStarterAssetsInputState(bool lockCursor, bool allowLookInput)
    {
        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = FindFirstObjectByType<StarterAssetsInputs>();
        }

        if (starterAssetsInputs == null)
        {
            return;
        }

        starterAssetsInputs.cursorLocked = lockCursor;
        starterAssetsInputs.cursorInputForLook = allowLookInput;
        starterAssetsInputs.MoveInput(Vector2.zero);
        starterAssetsInputs.LookInput(Vector2.zero);
        starterAssetsInputs.JumpInput(false);
        starterAssetsInputs.SprintInput(false);
    }
}

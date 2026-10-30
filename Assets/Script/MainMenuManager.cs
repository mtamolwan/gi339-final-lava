using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject creditCanvas;

    private void Start()
    {
        // Make sure credit canvas is hidden at start
        if (creditCanvas != null)
            creditCanvas.SetActive(false);
    }

    // --- Assign to your START button ---
    public void OnStartButton()
    {
        SceneManager.LoadScene("lvl_1_bedroom");
    }

    // --- Assign to your CREDIT button ---
    public void OnCreditButton()
    {
        if (creditCanvas != null)
            creditCanvas.SetActive(true);
    }

    // --- Assign to a BACK / CLOSE button inside the credit canvas ---
    public void OnCloseCreditButton()
    {
        if (creditCanvas != null)
            creditCanvas.SetActive(false);
    }

    // --- Assign to your QUIT button ---
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
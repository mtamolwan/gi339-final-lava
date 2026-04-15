using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; 

    void Start()
    {
        
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void SetupGameOver()
    {
        gameOverPanel.SetActive(true); 
        Time.timeScale = 0f;          
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
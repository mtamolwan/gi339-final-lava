using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestarter : MonoBehaviour
{
    
    public void RestartCurrentLevel()
    {
        
        Time.timeScale = 1f; 
        
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
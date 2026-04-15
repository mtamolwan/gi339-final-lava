using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public GameOverManager gameOverManager; 

    

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gameOverManager.SetupGameOver();
        }
    }
    
}
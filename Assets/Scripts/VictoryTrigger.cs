using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    public string victorySceneName = "Banana";
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.finalScore = GameManager.Instance != null ? GameManager.Instance.GetScore() : 0;
            SceneManager.LoadScene(victorySceneName); 
            
        }
    }
}

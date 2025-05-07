using UnityEngine;


public class DeathZone : MonoBehaviour
{
    public float Timer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Die();

            // Llamar GameOver tras un retraso para dejar que la animación se reproduzca
            other.GetComponent<PlayerController>().StartCoroutine(WaitAndTriggerGameOver());
        }
    }

    private System.Collections.IEnumerator WaitAndTriggerGameOver()
    {
        yield return new WaitForSeconds(Timer); // duración de animación
        GameManager.Instance.GameOver();
    }
}

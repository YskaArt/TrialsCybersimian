using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Animator anim = other.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Death"); 
            }

            other.GetComponent<PlayerController>().enabled = false;

            // Llamar GameOver tras un retraso para dejar que la animación se reproduzca
            other.GetComponent<PlayerController>().StartCoroutine(WaitAndTriggerGameOver());
        }
    }

    private System.Collections.IEnumerator WaitAndTriggerGameOver()
    {
        yield return new WaitForSeconds(1.5f); // duración de animación
        GameManager.Instance.GameOver();
    }
}

using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 100;
    public float rotationSpeed = 100f; // grados por segundo

    private void Update()
    {
        // Rota la moneda alrededor del eje Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}

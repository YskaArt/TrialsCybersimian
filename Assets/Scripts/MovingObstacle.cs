using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    private void Update()
    {
        // Movimiento entre punto A y punto B
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(pointA, pointB, t);
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja las posiciones A y B en el editor para visualizarlas
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointA, 0.2f);
        Gizmos.DrawSphere(pointB, 0.2f);
        Gizmos.DrawLine(pointA, pointB);
    }
}

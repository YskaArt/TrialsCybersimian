using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3();
    public float mouseSensitivity = 3f;
    public float rotationSmoothTime = 0.1f;

    private float currentYaw;
    private float currentPitch;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    public float minPitch = -30f;
    public float maxPitch = 60f;

    void LateUpdate()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);


        Vector3 targetRotation = new Vector3(currentPitch, currentYaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);


        transform.position = target.position + Quaternion.Euler(currentRotation.x, currentRotation.y, 0) * offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);


        Vector3 lookDir = transform.position - target.position;
        lookDir.y = 0;
        target.rotation = Quaternion.LookRotation(-lookDir);
    }
}

using UnityEngine;

public class RotatingObject : MonoBehaviour
{
     public float rotationAngle = 30.0f; // Maximale Rotation in Grad
    public float speed = 2.0f; // Geschwindigkeit der Rotation

    private float startRotation;
    private float targetRotation;
    private bool rotatingForward = true;

    void Start()
    {
        startRotation = transform.eulerAngles.z;
        targetRotation = startRotation + rotationAngle;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        float newRotation;

        if (rotatingForward)
        {
            newRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetRotation, step);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, newRotation));
            if (Mathf.Approximately(newRotation, targetRotation))
            {
                rotatingForward = false;
                targetRotation = startRotation - rotationAngle;
            }
        }
        else
        {
            newRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetRotation, step);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, newRotation));
            if (Mathf.Approximately(newRotation, targetRotation))
            {
                rotatingForward = true;
                targetRotation = startRotation + rotationAngle;
            }
        }
    }
}

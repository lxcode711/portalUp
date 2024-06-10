using UnityEngine;

public class MovingPlatformLR : MonoBehaviour
{
    public float speed = 10f; // Geschwindigkeit der Plattform
    public float distance = 20f; // Distanz, die die Plattform zurücklegt
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 newPos = startPos; 
        newPos.x += Mathf.PingPong(Time.time * speed, distance) - (distance / 2f); // Berechnung der neuen Position
        transform.position = newPos;
    }
}

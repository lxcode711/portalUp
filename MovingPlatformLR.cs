using UnityEngine;

public class MovingPlatformLR : MonoBehaviour
{
    public float speed = 10f;
    public float distance = 20f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 newPos = startPos;
        newPos.x += Mathf.PingPong(Time.time * speed, distance) - (distance / 2f);
        transform.position = newPos;
    }
}

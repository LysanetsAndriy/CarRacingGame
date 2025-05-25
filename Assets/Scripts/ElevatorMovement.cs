using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    public float speed = 2.0f; // Speed of the bridge movement
    public float startingYPosition = -0.5f; // Starting Y position of the bridge
    public float endingYPosition = 40.0f; // Ending Y position of the bridge
    // move the bridge only on y axis
    void Start()
    {
        // Set the initial position of the bridge
        transform.position = new Vector3(transform.position.x, startingYPosition + 10.0f, transform.position.z);
    }
    void Update()
    {
        // Move the elevator up and down
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // If the elevator reaches the ending position, make it go back to the starting position
        if (transform.position.y >= endingYPosition)
        {
            speed = -speed;
        }
        else if (transform.position.y <= startingYPosition)
        {
            speed = -speed;
        }
        
    }
}

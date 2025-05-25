using UnityEngine;

public class BridgeMovement : MonoBehaviour
{
    public float speed = 2.0f; // Speed of the bridge movement
    public float startingZPosition = -1105.0f; // Starting Z position of the bridge
    public float endingZPosition = -1175.0f; // Ending Z position of the bridge
    // move the bridge only on z axis
    void Start()
    {
        // Set the initial position of the bridge
        transform.position = new Vector3(transform.position.x, transform.position.y, startingZPosition + 10.0f * Mathf.Sign(startingZPosition));
    }
    void Update()
    {
        // Move the bridge forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // If the bridge reaches the ending position, make it go back to the starting position
        if (Mathf.Abs(transform.position.z) >= Mathf.Abs(endingZPosition))
        {
            // Reverse the direction of the bridge
            speed = -speed;
        }
        else if (Mathf.Abs(transform.position.z) <= Mathf.Abs(startingZPosition))
        {
            // Reverse the direction of the bridge
            speed = -speed;
        }
        
    }
}

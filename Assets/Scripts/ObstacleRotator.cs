using UnityEngine;

public class ObstacleRotator : MonoBehaviour
{
    public float rotationSpeed = 1.5f;
    
    void Update() 
    {
        transform.Rotate(new Vector3(0, 0, -45) * Time.deltaTime * rotationSpeed);
    }
}

using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPointId;
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car == null) return;

        // tell the car to update its “last checkpoint” & lap count
        car.SetLastCheckPoint(transform);

        // inform the manager (who will detect finish)
        RaceManager.Instance.CarCrossedCheckpoint(car, checkPointId);
    }
    
}

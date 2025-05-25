using UnityEngine;
using UnityEngine.UI;

public class RaceUIController : MonoBehaviour
{
    public CarController car;
    public Text infoText;
    public RaceManager raceManager;
    public Text resultText;   

    void Update()
    {
        // compute each car's progress
        float p1 = raceManager.GetProgress(raceManager.car1);
        float p2 = raceManager.GetProgress(raceManager.car2);
        Debug.Log($"Car 1 progress: {p1}, Car 2 progress: {p2}");

        // Determine this car's place
        bool amCar1 = (car == raceManager.car1);
        float myProgress = amCar1 ? p1 : p2;
        float otherProgress = amCar1 ? p2 : p1;
        int place = myProgress > otherProgress ? 1 : 2;

        // build the display strings
        int lap = car.currentLap;
        int maxLap = raceManager.totalLaps;
        int cp = car.currentCheckpointIndex + 1;
        int maxCp = raceManager.CheckpointCount;

        string placeSuffix = place == 1 ? "st" : "nd";
        infoText.text =
            $"Place: {place}{placeSuffix}\n" +
            $"Lap: {lap}/{maxLap}\n" +
            $"CP: {cp}/{maxCp}\n";

    }
    
    public void ShowVictory()
    {
        resultText.text = "You Win!";
        resultText.color = Color.green;    
        resultText.gameObject.SetActive(true);
    }

    public void ShowDefeat()
    {
        resultText.text = "You Lose!";
        resultText.color = Color.red;     
        resultText.gameObject.SetActive(true);
    }
}

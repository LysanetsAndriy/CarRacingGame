using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }
    [Header("Cars")]
    public CarController car1;
    public CarController car2;

    [Header("Cameras")]
    public CameraFollow camera1Follow;
    public CameraFollow camera2Follow;

    [Header("Checkpoints")]
    public CheckPoint[] checkpointObjs; // Array of CheckPoint objects
    public int startCheckpointId = 0; // ID of the starting checkpoint
    public int finishCheckpointId = 0; // ID of the finish line checkpoint
    public int totalLaps = 3; // Total number of laps

    private Transform[] checkpoints; // Sorted by ID
    private int cpCount;
    public int CheckpointCount => cpCount;
    public Transform startTransform;
    private bool raceFinished = false;
    private CarController firstFinisher = null;
    [SerializeField] private Scenes menuScene;

    private string currentSceneName;
    [Header("Car Prefabs")]
    public List<GameObject> carPrefabs;

    [Header("Players Info")]
    public RaceUIController player1Info;
    public RaceUIController player2Info;

    private float trackLength;
    private float[] segmentLengths;
    private float[] cumulativeLengths;


    void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // sort checkpoints by ID
        checkpoints = checkpointObjs
            .OrderBy(cp => cp.checkPointId)
            .Select(cp => cp.transform)
            .ToArray();
        cpCount = checkpoints.Length;

        startTransform = checkpoints.First(t => t.GetComponent<CheckPoint>().checkPointId == startCheckpointId);

        cpCount = checkpoints.Length;
        segmentLengths    = new float[cpCount];
        cumulativeLengths = new float[cpCount + 1]; 

        // fill segmentLengths and cumulativeLengths
        cumulativeLengths[0] = 0f;
        for (int i = 0; i < cpCount; i++)
        {
            int next = (i + 1) % cpCount;
            float len = Vector3.Distance(
                checkpoints[i].position,
                checkpoints[next].position);
            segmentLengths[i] = len;
            cumulativeLengths[i + 1] = cumulativeLengths[i] + len;
        }

        trackLength = cumulativeLengths[cpCount];
        startTransform = checkpoints.First(t =>
            t.GetComponent<CheckPoint>().checkPointId == startCheckpointId);
    }
    void Start()
    {
        int sel1 = PlayerPrefs.GetInt("SelectedCar1", 0);
        int sel2 = PlayerPrefs.GetInt("SelectedCar2", 1);

        GameObject go1 = Instantiate(carPrefabs[sel1]);
        GameObject go2 = Instantiate(carPrefabs[sel2]);

        car1 = go1.GetComponent<CarController>();
        car2 = go2.GetComponent<CarController>();

        // assign input schemes
        car1.controlScheme = CarController.ControlScheme.WASD;
        car2.controlScheme = CarController.ControlScheme.ArrowKeys;

        // give each camera its target
        camera1Follow.carTarget = car1.transform;
        camera2Follow.carTarget = car2.transform;

        // assign UI
        player1Info.car = car1;
        player2Info.car = car2;

        // spawn at start line
        car1.SetLastCheckPoint(startTransform);
        car1.RespawnAtLastCheckpoint();
        car2.SetLastCheckPoint(startTransform);
        car2.RespawnAtLastCheckpoint();
    }

    public void CarCrossedCheckpoint(CarController car, int cpId)
    {
        // Debug.Log($"Car {car.name} crossed checkpoint {cpId}, current lap: {car.currentLap}, total laps: {totalLaps}");

        if (cpId == finishCheckpointId && car.currentLap + 1 > totalLaps)
        {
            if (!raceFinished)
            {
                raceFinished = true;
                firstFinisher = car;
                AnnounceWin(car);
            }
            else if (car != firstFinisher)
            {
                AnnounceLoss(car);
            }

        }
    }
    public void Update()
    {
        ReturnToMenu();
    }

    void AnnounceWin(CarController winner)
    {
        bool isP1 = (winner == car1);

        // Disable driving
        winner.enabled = false;
        winner.Stop();

        // Show on the correct UI
        if (isP1)
        {
            player1Info.ShowVictory();
            player2Info.ShowDefeat();
        }
        else
        {
            player2Info.ShowVictory();
            player1Info.ShowDefeat();
        }
    }
    void AnnounceLoss(CarController loser)
    {
        string name = (loser == car1) ? "Player 1" : "Player 2";
        loser.enabled = false;
        loser.Stop();
    }

    public float GetProgress(CarController car)
    {
        int idx = car.currentCheckpointIndex;
        int next = (idx + 1) % cpCount;

        Vector3 a = checkpoints[idx].position;
        Vector3 b = checkpoints[next].position;
        Vector3 ab = b - a;

        // Project the car's position onto segment AB to get distance along it
        Vector3 ap = car.transform.position - a;
        float proj = Vector3.Dot(ap, ab.normalized);
        float distAlongSegment = Mathf.Clamp(proj, 0f, ab.magnitude);

        // Distance so far this lap:
        float lapDistance = cumulativeLengths[idx] + distAlongSegment;

        // Total = full laps * lap length + this lap's distance
        return car.currentLap * trackLength + lapDistance;
    }

    public void ReturnToMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync(menuScene.ToString());
        }
    }
}
 
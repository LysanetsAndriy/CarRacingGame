using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class CarSelectionUI : MonoBehaviour
{
    [Header("UI")]
    public Dropdown player1Dropdown;
    public Dropdown player2Dropdown;
    public Button startButton;

    [Header("Cars")]
    public List<GameObject> carPrefabs;

    private int p1Index = 0;
    private int p2Index = 0;
    [SerializeField] private Scenes gameScene;

    void Start()
    {
        List<string> labels = carPrefabs
            .Select(prefab => prefab.name)
            .ToList();

        player1Dropdown.ClearOptions();
        player2Dropdown.ClearOptions();

        player1Dropdown.AddOptions(labels);
        player2Dropdown.AddOptions(labels);

        player1Dropdown.onValueChanged.AddListener(i => p1Index = i);
        player2Dropdown.onValueChanged.AddListener(i => p2Index = i);

        startButton.onClick.AddListener(OnStartClicked);
    }

    void OnStartClicked()
    {
        PlayerPrefs.SetInt("SelectedCar1", p1Index);
        PlayerPrefs.SetInt("SelectedCar2", p2Index);
        PlayerPrefs.Save();

        SceneManager.LoadSceneAsync(gameScene.ToString());
    }
}

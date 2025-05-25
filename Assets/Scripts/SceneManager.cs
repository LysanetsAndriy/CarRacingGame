using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private Scenes _nextScene;

    private string _currentSceneName;

    private void Awake()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(_nextScene.ToString());
    }
}


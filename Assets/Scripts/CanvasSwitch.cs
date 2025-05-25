using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasSwitch : MonoBehaviour
{
    [SerializeField] private Canvas _activeCanvas;
    [SerializeField] private Canvas _nextCanvas;

    private void Awake()
    {
        CheckFields();
    }

    private void OnValidate()
    {
        CheckFields();
    }

    public void SwitchCanvas()
    {
        if (_activeCanvas == null || _nextCanvas == null)
        {
            Debug.LogError($"Active or next canvas at {this.gameObject.name} was not assigned!");
            return;
        }
        else
        {
            _activeCanvas.gameObject.SetActive(false);
            _nextCanvas.gameObject.SetActive(true);
        }

    }

    private void CheckFields()
    {
        if (_activeCanvas == null)
        {
            Debug.LogError($"Active canvas {this.gameObject.name} was not assigned!");
            _activeCanvas = GetComponent<Canvas>();
        }

        if (_nextCanvas == null)
        {
            Debug.LogError($"Next canvas {this.gameObject.name} was not assigned!");
        }
    }
}

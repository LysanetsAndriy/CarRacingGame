using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas mainMenuCanvas;
    public Canvas aboutCanvas;
    public Canvas carSelectCanvas;

    void Awake()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        aboutCanvas.gameObject.SetActive(false);
        carSelectCanvas.gameObject.SetActive(false);
    }

    // Called by the “Про автора” button
    public void ShowAbout()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        aboutCanvas.gameObject.SetActive(true);
    }

    // Called by the “Почати” button
    public void ShowCarSelect()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        carSelectCanvas.gameObject.SetActive(true);
    }

    public void ShowMainMenu()
    {
        aboutCanvas.gameObject.SetActive(false);
        carSelectCanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }
}

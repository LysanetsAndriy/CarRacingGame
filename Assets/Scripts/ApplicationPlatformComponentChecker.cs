using UnityEngine;

public class ApplicationPlatformComponentChecker : MonoBehaviour
{
    private void Awake()
    {
        DeactivateObjectOnWeb();
    }

    private void DeactivateObjectOnWeb()
    {
        bool isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        bool isEditor = Application.isEditor;

        if (isWebGL || isEditor)
        {
            gameObject.SetActive(false);
        }
    }
}

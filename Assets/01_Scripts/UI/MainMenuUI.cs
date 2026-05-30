using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    SceneTransitionUI _sceneTransition;

    private void Start()
    {
        _sceneTransition = FindFirstObjectByType<SceneTransitionUI>();
    }
    public void StartGame()
    {
        _sceneTransition.DarkenScreen("MaxScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

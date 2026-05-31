using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    SceneTransitionUI _sceneTransition;

    private void Start()
    {
        _sceneTransition = FindFirstObjectByType<SceneTransitionUI>();
    }

    public void ResumeGame()
    {
        GameManager.Instance.UnPause();
        pauseMenuUI.SetActive(false);
    }

    public void MainMenu()
    {
        GameManager.Instance.EndGame();
        _sceneTransition.DarkenScreen("MainMenu");
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.Instance.State != GameState.Paused &&
            GameManager.Instance.State != GameState.Conversation)
        {
            GameManager.Instance.Pause();
            pauseMenuUI.SetActive(true);
        }
        else if (context.started && GameManager.Instance.State == GameState.Paused)
        {
            GameManager.Instance.UnPause();
            pauseMenuUI.SetActive(false);
        }
    }
}
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    SceneTransitionUI _sceneTransition;

    private void Start()
    {
        _sceneTransition = FindFirstObjectByType<SceneTransitionUI>();
    }
    
    public void ResumeGame()
    {
        AudioManager.Instance.ChangeBGM(AudioManager.Instance.creditsBGM);
        _sceneTransition.DarkenScreen("MainMenu");
    }
    
    public void MainMenu()
    {
        AudioManager.Instance.ChangeBGM(AudioManager.Instance.creditsBGM);
        _sceneTransition.DarkenScreen("MainMenu");
    }
}

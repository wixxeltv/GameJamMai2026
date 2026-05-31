using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    SceneTransitionUI _sceneTransition;

    private void Start()
    {
        _sceneTransition = FindFirstObjectByType<SceneTransitionUI>();
    }
    public void MainMenu()
    {
        _sceneTransition.DarkenScreen("MainMenu");
    }

    public void ContinueInfinitely() { }
}

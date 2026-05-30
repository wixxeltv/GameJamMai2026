using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private SceneTransitionUI sceneTransition;

    private void Start()
    {
        sceneTransition = FindFirstObjectByType<SceneTransitionUI>();
    }
    public void OnEnable()
    {
        scoreText.text = ScoreManager.Instance.GetTotalScore().ToString("00000");
    }
    
    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    
    public void MainMenu()
    {
        sceneTransition.DarkenScreen("MainMenu");
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
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
        SceneManager.LoadScene("MainMenu");
    }
}

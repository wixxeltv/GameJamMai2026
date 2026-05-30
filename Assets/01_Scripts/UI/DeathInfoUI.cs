using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public void OnEnable()
    {
        scoreText.text = ScoreManager.Instance.GetTotalScore().ToString();
    }
    
    public void StartGame()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }
    
    public void MainMenu()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}

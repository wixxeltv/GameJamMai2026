using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _totalScore = 0;
    
    public static ScoreManager Instance;
    
    public int GetTotalScore() { return _totalScore; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void IncreaseScore(int score)
    {
        _totalScore += score;
    }
}

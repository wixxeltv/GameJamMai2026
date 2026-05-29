using TMPro;
using UnityEngine;

public class ScoreInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalScoreText;

    void Update()
    {
        totalScoreText.text = ScoreManager.Instance.GetTotalScore().ToString("00000");
    }
}

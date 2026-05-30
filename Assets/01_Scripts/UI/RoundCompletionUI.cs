using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundCompletionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Image completionPanel;

    private int _roundcount;
    private WaveManager _waveManager;
    
    private void Start()
    {
        _waveManager=WaveManager.Instance;
        
        _waveManager.OnWaveStarted.AddListener(OnWaveStarted);
        _waveManager.OnWaveTimerTick.AddListener(OnWaveTimerTick);
    }
    
    private void OnWaveStarted(int round)
    {
        Debug.Log("Wave has been called");
        completionPanel.gameObject.SetActive(false);
        _roundcount = round;
    }
    
    private void OnWaveTimerTick(float waitTime)
    {
        Debug.Log("Timer has been called");
        completionPanel.gameObject.SetActive(true);
        roundText.text="Round "+(_roundcount+1)+" Completed";
        countdownText.text=((int)waitTime).ToString();
    }
}

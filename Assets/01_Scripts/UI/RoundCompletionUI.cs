using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundCompletionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Image completionPanel;

    private WaveManager _waveManager;
    
    private void Start()
    {
        _waveManager=WaveManager.Instance;
        
        _waveManager.OnWaveStarted.AddListener(OnWaveStarted);
        _waveManager.OnWaveTimerTick.AddListener(OnWaveTimerTick);
    }
    
    public void OnWaveStarted(int round)
    {
        completionPanel.gameObject.SetActive(false);
        roundText.text="Round "+(round+1)+" Completed";
    }
    
    public void OnWaveTimerTick(float waitTime)
    {
        completionPanel.gameObject.SetActive(true);
        countdownText.text=((int)waitTime).ToString();
    }
}

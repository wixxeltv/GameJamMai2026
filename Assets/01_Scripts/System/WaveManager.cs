using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private int numberOfRounds;
    public int currentWave;
    
    private WaveInfoUI _waveInfoUI;

    private void Start()
    {
        _waveInfoUI = FindFirstObjectByType<WaveInfoUI>();
        StartWave();
    }
    
    public void StartWave()
    {
        _waveInfoUI.SetWaveCount(currentWave);
    }

    public void EndWave()
    {
        currentWave++;
    }
    
}

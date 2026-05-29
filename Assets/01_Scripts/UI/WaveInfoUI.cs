using System;
using TMPro;
using UnityEngine;

public class WaveInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveCountText;

    public void SetWaveCount(int waveCount)
    {
        waveCountText.text=waveCount+1.ToString();
    }
}

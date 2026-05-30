using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject wavePanel;
    [SerializeField] private TextMeshProUGUI waveCountText;
    
    [Header("Animation Settings")]
    [SerializeField] private float maxScaleMultiplier = 1.1f;
    [SerializeField] private float animationDuration = 1.0f;
    
    private Vector3 _originalScale;

    private void Start()
    {
        _originalScale = wavePanel.transform.localScale;
    }
    
    public void SetWaveCount(int waveCount)
    {
        waveCountText.text=(waveCount+1).ToString();

        if (waveCount == 0) return;
        
        StartCoroutine(PulsePanelAnimation());
    }
    
    private IEnumerator PulsePanelAnimation()
    {
        float elapsedTime = 0f;
        Vector3 targetScale = _originalScale * maxScaleMultiplier;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            float pulseWeight = Mathf.Sin(t * Mathf.PI);

            wavePanel.transform.localScale = Vector3.Lerp(_originalScale, targetScale, pulseWeight);

            yield return null;
        }
        wavePanel.transform.localScale = _originalScale;
    }
}

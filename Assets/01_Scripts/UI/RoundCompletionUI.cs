using System;
using System.Collections;
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
    private Coroutine _fadeCoroutine;
    private float _transparency;
    
    private void Start()
    {
        _waveManager=WaveManager.Instance;
        
        _waveManager.OnWaveStarted.AddListener(OnWaveStarted);
        _waveManager.OnWaveTimerTick.AddListener(OnWaveTimerTick);
        _transparency = completionPanel.color.a;
    }

    public void Update()
    {
        if (_waveManager._waitingForDialogue)
        {
            completionPanel.gameObject.SetActive(false);
        }
    }

    private void OnWaveStarted(int round)
    {
        Debug.Log("Wave has been called");
        completionPanel.gameObject.SetActive(false);
        _roundcount = round;
    }
    
    private void OnWaveTimerTick(float waitTime)
    {
        if (_fadeCoroutine == null && waitTime > 4.5f)
        {
            _fadeCoroutine = StartCoroutine(DoFadeOut());
        }
        else if (_fadeCoroutine == null && waitTime < 0.95f)
        {
            _fadeCoroutine = StartCoroutine(DoFadeIn());
        }
        Debug.Log("Timer has been called");
        completionPanel.gameObject.SetActive(true);
        roundText.text="Round "+(_roundcount)+" Completed";
        countdownText.text=((int)waitTime).ToString();
    }
    
    private IEnumerator DoFadeOut()
    {
        Color startColor = new Color(completionPanel.color.r, completionPanel.color.g, completionPanel.color.b, 0);;
        Color endColor = new Color(completionPanel.color.r, completionPanel.color.g, completionPanel.color.b, _transparency); // fully opaque

        float t = 0f;

        while (t < 1f)
        {
            completionPanel.color = Color.Lerp(startColor, endColor, t);
            t += Time.deltaTime / 1;
            yield return null;
        }
        _fadeCoroutine = null;
    }

    private IEnumerator DoFadeIn()
    {
        Color startcolor = new Color(completionPanel.color.r, completionPanel.color.g, completionPanel.color.b, _transparency);;
        Color endcolor = new Color(completionPanel.color.r, completionPanel.color.g, completionPanel.color.b, 0);


        float elapsed = 0f;
        
        while (elapsed < 1)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1;
            completionPanel.color = Color.Lerp(startcolor, endcolor, t);
            yield return null;
        }
        _fadeCoroutine = null;
    }
}

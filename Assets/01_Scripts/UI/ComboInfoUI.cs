using System.Collections.Generic;
using UnityEngine;

public class ComboInfoUI : MonoBehaviour
{
    //0 - Red | 1 - Blue | 2 - Yellow
    [SerializeField] ComboSlotUI[] comboSlots;

    //0 - Up | 1 - Right | 2 - Down | 3 - Left
    [SerializeField] Sprite[] arrowImages;

    private ComboSystem _comboSystem;
    private WaveManager _waveManager;
    
    [SerializeField] private AudioClip confirmationSFX;
    [SerializeField] private AudioClip[] voicelinesSFX;

    private void Awake()
    {
        _comboSystem = FindFirstObjectByType<ComboSystem>();
        _waveManager = FindFirstObjectByType<WaveManager>();
        
        foreach (var slot in comboSlots)
        {
            slot.SetArrow(arrowImages);
        }
        _comboSystem.OnCombosGenerated.AddListener(OnCombosGenerated);
        _comboSystem.OnBufferChanged.AddListener(OnBufferChanged);
        _comboSystem.OnColorSwitched.AddListener(OnColorSwitched);
        _waveManager.OnWaveStarted.AddListener(OnValueReset);
    }

    public void OnColorSwitched(ColorType color)
    {
        int randomIndex = Random.Range(0, voicelinesSFX.Length);
        AudioManager.Instance.PlaySfx(voicelinesSFX[randomIndex], 100f);
        AudioManager.Instance.PlaySfx(confirmationSFX, 100f);
    }
    private void OnCombosGenerated(Dictionary<ColorType, List<ComboSystem.ComboType>> combos)
    {
        foreach (var key in combos)
        {
            switch (key.Key)
            {
                case ColorType.Red:
                    comboSlots[0].UpdateSlots(key.Value);
                    break;
                case ColorType.Blue:
                    comboSlots[1].UpdateSlots(key.Value);
                    break;
                case ColorType.Yellow:
                    comboSlots[2].UpdateSlots(key.Value);
                    break;
            }
        }
    }
    
    private void OnBufferChanged(List<ComboSystem.ComboType> buffer)
    {
        // Update all combo slots with the current buffer
        foreach (var slot in comboSlots)
        {
            slot.UpdateBuffer(buffer);
        }
    }
    
    private void OnValueReset(int round)
    {
        if(round != 0) return;
        
        // Update all combo slots with the current buffer
        foreach (var slot in comboSlots)
        {
            slot.ResetSlots();
        }
    }
}
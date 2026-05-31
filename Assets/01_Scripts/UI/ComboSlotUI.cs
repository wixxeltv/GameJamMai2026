using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboSlotUI : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private Image[] arrowSlots;
    [SerializeField] private Image[] comboSlots;
    [SerializeField] private RectTransform backgroundImage;
    [Header("Images")]
    private Sprite[] _arrowImages;
    
    [Header("Colors")]
    [SerializeField] private Color unpressedColor = Color.white;
    [SerializeField] private Color pressedColor = Color.grey;
    
    [SerializeField] private AudioClip inputPressedSFX;

    private List<ComboSystem.ComboType> _currentCombo;
    private int increaseValue = 55;
    private Vector2 originalSize;
    private int _previousBufferCount = 0;

    private void Start()
    {
        originalSize = backgroundImage.sizeDelta;
    }

    public void SetArrow(Sprite[] arrows)
    {
        _arrowImages = arrows;
    }

    public void UpdateSlots(List<ComboSystem.ComboType> combos)
    {
        _currentCombo = combos;
        
        Vector2 size = originalSize;
        if (combos.Count > 1)
        {
            size.x += increaseValue * (combos.Count - 1);
        }
        backgroundImage.sizeDelta = size;
        
        foreach (var slot in arrowSlots)
        {
            slot.gameObject.SetActive(false);
            slot.color = unpressedColor;
        }
        foreach (var slot in comboSlots)
        {
            slot.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < combos.Count; i++)
        {
            arrowSlots[i].gameObject.SetActive(true);
            comboSlots[i].gameObject.SetActive(true);
            
            switch (combos[i])
            {
                case ComboSystem.ComboType.ArrowUp:
                    comboSlots[i].sprite = _arrowImages[0];
                    break;
                case ComboSystem.ComboType.ArrowRight:
                    comboSlots[i].sprite = _arrowImages[1];
                    break;
                case ComboSystem.ComboType.ArrowDown:
                    comboSlots[i].sprite = _arrowImages[2];
                    break;
                case ComboSystem.ComboType.ArrowLeft:
                    comboSlots[i].sprite = _arrowImages[3];
                    break;
            }
        }
    }
    
    public void UpdateBuffer(List<ComboSystem.ComboType> buffer)
    {
        if (_currentCombo == null) return;
        
        if (buffer.Count != _previousBufferCount)
        {
            AudioManager.Instance.PlaySfx(inputPressedSFX, 100f);
            _previousBufferCount = buffer.Count;
        }
        
        for (int i = 0; i < arrowSlots.Length; i++)
        {
            if (arrowSlots[i].gameObject.activeSelf)
                arrowSlots[i].color = unpressedColor;
        }
        
        int matches = 0;
        int offset = buffer.Count - _currentCombo.Count;
        if (offset < 0) offset = 0;
        
        for (int i = 0; i < _currentCombo.Count && (offset + i) < buffer.Count; i++)
        {
            if (buffer[offset + i] == _currentCombo[i])
            {
                matches = i + 1;
            }
            else
            {
                break;
            }
        }
        
        for (int i = 0; i < matches && i < arrowSlots.Length; i++)
        {
            arrowSlots[i].color = pressedColor;
        }
    }

    public void ResetSlotColors()
    {
        for (int i = 0; i < arrowSlots.Length; i++)
        {
            if (arrowSlots[i].gameObject.activeSelf)
                arrowSlots[i].color = unpressedColor;
        }
    }

    public void ResetSlots()
    {
        backgroundImage.sizeDelta = originalSize;
        _previousBufferCount = 0;
        
        for (int i = 0; i < arrowSlots.Length; i++)
        {
            arrowSlots[i].gameObject.SetActive(false);
            arrowSlots[i].color = unpressedColor;
        }
        
        for (int i = 0; i < comboSlots.Length; i++)
        {
            comboSlots[i].gameObject.SetActive(false);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    public void SetArrow(Sprite[] arrows)
    {
        _arrowImages = arrows;
    }

    public void UpdateSlots(List<ComboSystem.ComboType> combos)
    {
        _currentCombo = combos;
        if (combos.Count > 1)
        {
            Vector2 size = backgroundImage.sizeDelta;
            backgroundImage.sizeDelta = new Vector2(size.x+55, size.y);
        }
        for (int i = 0; i < combos.Count; i++)
        {
            arrowSlots[i].gameObject.SetActive(true);
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
        Debug.Log("update buffer");
        AudioManager.Instance.PlaySfx(inputPressedSFX, 100f);
        if (_currentCombo == null) return;
        
        for (int i = 0; i < _currentCombo.Count && i < arrowSlots.Length; i++)
        {
            arrowSlots[i].color = unpressedColor;
        }
        
        int matches = 0;
        int offset = buffer.Count - _currentCombo.Count;
        if (offset < 0) offset = 0;
        
        for (int i = 0; i < _currentCombo.Count && (offset + i) < buffer.Count; i++)
        {
            if (buffer[offset + i] == _currentCombo[i])
            {
                matches++;
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
}
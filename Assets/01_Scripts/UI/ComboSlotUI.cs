using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboSlotUI : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private Image[] comboSlots;
    [Header("Images")]
    private Sprite[] _arrowImages;
    
    [Header("Colors")]
    [SerializeField] private Color unpressedColor;
    [SerializeField] private Color pressedColor;

    public void SetArrow(Sprite[] arrows)
    {
        _arrowImages = arrows;
    }

    public void UpdateSlots(List<ComboSystem.ComboType> combos)
    {
        // Then fill only the ones we have combos for
        for (int i = 0; i < combos.Count; i++)
        {
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
}
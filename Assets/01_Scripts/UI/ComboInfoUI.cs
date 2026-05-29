using System.Collections.Generic;
using UnityEngine;

public class ComboInfoUI : MonoBehaviour
{
    //0 - Red | 1 - Blue | 2 - Yellow
    [SerializeField] ComboSlotUI[] comboSlots;

    //0 - Up | 1 - Right | 2 - Down | 3 - Left
    [SerializeField] Sprite[] arrowImages;

    private ComboSystem _comboSystem;

    private void Awake()
    {
        _comboSystem = FindFirstObjectByType<ComboSystem>();
        foreach (var slot in comboSlots)
        {
            slot.SetArrow(arrowImages);
        }
        _comboSystem.OnCombosGenerated.AddListener(OnCombosGenerated);
    }

    void Start()
    {
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
}
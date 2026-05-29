using System.Collections.Generic;
using UnityEngine;

public class ComboSystemTester : MonoBehaviour
{
    private ComboSystem _comboSystem;

    private void Awake()
    {
        _comboSystem = GetComponent<ComboSystem>();

        _comboSystem.OnCombosGenerated.AddListener(OnCombosGenerated);
        _comboSystem.OnBufferChanged.AddListener(OnBufferChanged);
        _comboSystem.OnColorSwitched.AddListener(OnColorSwitched);
        _comboSystem.OnBufferReset.AddListener(() => Debug.Log("Buffer reset (timeout)"));
    }

    private void OnCombosGenerated(Dictionary<ColorType, List<ComboSystem.ComboType>> combos)
    {
        Debug.Log("=== NOUVEAUX COMBOS ===");
        foreach (var kvp in combos)
            Debug.Log($"{kvp.Key} → {string.Join(", ", kvp.Value)}");
    }

    private void OnBufferChanged(List<ComboSystem.ComboType> buffer)
    {
        Debug.Log($"Buffer : [{string.Join(", ", buffer)}]");
    }

    private void OnColorSwitched(ColorType color)
    {
        Debug.Log($"✓ COMBO RÉUSSI → couleur : {color}");
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ComboSystem : MonoBehaviour
{
    public enum ComboType { ArrowUp, ArrowDown, ArrowLeft, ArrowRight }

    [Header("Config")]
    //[SerializeField] private int _startLength = 1;
    [SerializeField] private int _maxLength = 4;
    [SerializeField] private float _bufferResetTime = 2f;

    public UnityEvent<Dictionary<ColorType, List<ComboType>>> OnCombosGenerated;
    public UnityEvent<List<ComboType>> OnBufferChanged;
    public UnityEvent<ColorType> OnColorSwitched;
    public UnityEvent OnBufferReset;

    public Dictionary<ColorType, List<ComboType>> CurrentCombos => _combos;
    public List<ComboType> CurrentBuffer => _inputBuffer;

    private Dictionary<ColorType, List<ComboType>> _combos = new();
    private List<ComboType> _inputBuffer = new();
    private int _currentLength;
    private float _resetTimer;
    private PlayerColorController _colorController;

    private void Awake()
    {
        _colorController = GetComponent<PlayerColorController>();
    }

    private void Start() { }

    private void Update()
    {
        if (_inputBuffer.Count == 0) return;
        _resetTimer -= Time.deltaTime;
        if (_resetTimer <= 0f)
        {
            _inputBuffer.Clear();
            OnBufferReset?.Invoke();
        }
    }
    
    public void OnComboUp(InputAction.CallbackContext ctx)    { if (ctx.performed) AddInput(ComboType.ArrowUp); }
    public void OnComboDown(InputAction.CallbackContext ctx)  { if (ctx.performed) AddInput(ComboType.ArrowDown); }
    public void OnComboLeft(InputAction.CallbackContext ctx)  { if (ctx.performed) AddInput(ComboType.ArrowLeft); }
    public void OnComboRight(InputAction.CallbackContext ctx) { if (ctx.performed) AddInput(ComboType.ArrowRight); }

    public void GenerateCombos(int waveNumber)
    {
        _inputBuffer.Clear();

        ComboType[] allInputs = { ComboType.ArrowUp, ComboType.ArrowDown, ComboType.ArrowLeft, ComboType.ArrowRight };
        ColorType[] colors = { ColorType.Red, ColorType.Yellow, ColorType.Blue };

        if (waveNumber == 1)
        {
            _combos.Clear();
            foreach (ColorType color in colors)
                _combos[color] = RandomCombo(allInputs, 1);
        }
        else if (_currentLength < _maxLength)
        {
            foreach (ColorType color in colors)
                _combos[color].Add(allInputs[Random.Range(0, allInputs.Length)]);
        }

        _currentLength = _combos[ColorType.Red].Count;
        OnCombosGenerated?.Invoke(_combos);
    }

    private List<ComboType> RandomCombo(ComboType[] allInputs, int length)
    {
        List<ComboType> combo;
        int attempts = 0;
        do
        {
            combo = new List<ComboType>();
            for (int i = 0; i < length; i++)
                combo.Add(allInputs[Random.Range(0, allInputs.Length)]);
            attempts++;
        }
        while (attempts < 100 && IsComboAlreadyUsed(combo));
        return combo;
    }

    private void AddInput(ComboType input)
    {
        _inputBuffer.Add(input);
        _resetTimer = _bufferResetTime;

        if (_inputBuffer.Count > _currentLength)
            _inputBuffer.RemoveAt(0);

        OnBufferChanged?.Invoke(new List<ComboType>(_inputBuffer));
        CheckMatch();
    }

    private void CheckMatch()
    {
        foreach (var kvp in _combos)
        {
            List<ComboType> combo = kvp.Value;
            if (_inputBuffer.Count < combo.Count) continue;

            bool match = true;
            int offset = _inputBuffer.Count - combo.Count;
            for (int i = 0; i < combo.Count; i++)
            {
                if (_inputBuffer[offset + i] != combo[i]) { match = false; break; }
            }

            if (!match) continue;

            _colorController?.SwitchToColor(kvp.Key);
            _inputBuffer.Clear();
            OnBufferReset?.Invoke();
            OnColorSwitched?.Invoke(kvp.Key);
            return;
        }
    }

    private bool IsComboAlreadyUsed(List<ComboType> candidate)
    {
        foreach (var kvp in _combos)
        {
            if (kvp.Value.Count != candidate.Count) continue;
            bool same = true;
            for (int i = 0; i < candidate.Count; i++)
                if (kvp.Value[i] != candidate[i]) { same = false; break; }
            if (same) return true;
        }
        return false;
    }
}

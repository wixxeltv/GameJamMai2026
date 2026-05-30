using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerColorController _playerColorController;
    [SerializeField] private Image[] slots;
    private int _previousSelectedSlot;
    
    [Header("Animation Settings")]
    [SerializeField] private float maxScaleMultiplier = 1.1f;
    [SerializeField] private float animationDuration = 1.0f;

    private Vector3 _originalScale;
    
    void Start()
    {
        _playerColorController = FindFirstObjectByType<PlayerColorController>();
        _originalScale = Vector3.one;
    }
    private void Update()
    {
        slots[_previousSelectedSlot].color = Color.white;
        slots[_previousSelectedSlot].transform.localScale = Vector3.one;
        switch (_playerColorController.CurrentColor)
        {
            case ColorType.Red:
                slots[0].color = Color.gray;
                StartCoroutine(PulsePanelAnimation(slots[0].gameObject));
                _previousSelectedSlot = 0;
                break;
            case ColorType.Blue:
                slots[1].color = Color.gray;
                StartCoroutine(PulsePanelAnimation(slots[1].gameObject));
                _previousSelectedSlot = 1;
                break;
            case ColorType.Yellow:
                slots[2].color = Color.gray;
                StartCoroutine(PulsePanelAnimation(slots[2].gameObject));
                _previousSelectedSlot = 2;
                break;
        }
        
    }
    
    private IEnumerator PulsePanelAnimation(GameObject slot)
    {
        float elapsedTime = 0f;
        Vector3 targetScale = _originalScale * maxScaleMultiplier;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            float pulseWeight = Mathf.Sin(t * Mathf.PI);

            slot.transform.localScale = Vector3.Lerp(_originalScale, targetScale, pulseWeight);

            yield return null;
        }
        slot.transform.localScale = _originalScale;
    }
}

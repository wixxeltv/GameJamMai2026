using UnityEngine;
using UnityEngine.UI;

public class UpDownArrowUI : MonoBehaviour
{
    [SerializeField] private Image arrow;
    [SerializeField] private float bounceHeight = 20f;
    [SerializeField] private float bounceSpeed = 2f;
    
    private float _originalY;
    
    void Start()
    {
        if (arrow != null)
        {
            _originalY = arrow.rectTransform.anchoredPosition.y;
        }
    }
    
    void Update()
    {
        if (arrow == null) return;
        
        Vector2 pos = arrow.rectTransform.anchoredPosition;
        pos.y = _originalY + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        arrow.rectTransform.anchoredPosition = pos;
    }
}
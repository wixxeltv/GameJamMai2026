using UnityEngine;
using UnityEngine.UI;

public class UpDownArrowUI : MonoBehaviour
{
    [SerializeField] private Image arrow;
    [SerializeField] private float bounceHeight = 20f;
    [SerializeField] private float bounceSpeed = 2f;
    
    void Update()
    {
        if (arrow == null) return;
        
        Vector2 pos = arrow.rectTransform.anchoredPosition;
        pos.y = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        arrow.rectTransform.anchoredPosition = pos;
    }
    
}

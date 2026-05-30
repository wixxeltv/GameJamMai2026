using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public float minimum;
    public float maximum;
    public float current;
    
    [Header("Settings")]
    [SerializeField] private float fillSpeed = 0.5f;
    private float _targetFillRatio;

    private void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        if (slider == null) return;
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        slider.value = Mathf.MoveTowards(slider.value, fillAmount, fillSpeed * Time.deltaTime);
    }
}

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public int minimum;
    public int maximum;
    public int current;
    
    [Header("Settings")]
    [SerializeField] private float fillSpeed = 0.5f;
    private float _targetFillRatio;

    private void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        slider.value = Mathf.MoveTowards(slider.value, fillAmount, fillSpeed * Time.deltaTime);
    }
}

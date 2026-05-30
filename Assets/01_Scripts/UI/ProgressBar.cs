using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image mask;
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
        mask.fillAmount = Mathf.MoveTowards(mask.fillAmount, fillAmount, fillSpeed * Time.deltaTime);
    }
}

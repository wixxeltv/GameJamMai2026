using UnityEngine;
using UnityEngine.UI;

public class EnemyProgressBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _fillSpeed = 2f;

    private float _targetFill = 1f;

    public void SetValues(float current, float maximum)
    {
        _targetFill = current / maximum;
    }

    private void Update()
    {
        if (_fillImage == null) return;
        _fillImage.fillAmount = Mathf.MoveTowards(_fillImage.fillAmount, _targetFill, _fillSpeed * Time.deltaTime);
    }
}

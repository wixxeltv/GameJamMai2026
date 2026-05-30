using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreInfoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private Image flashImage;
    
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1.0f;
    void Start()
    {
        ScoreManager.Instance.OnScoreUpdated.AddListener(UpdateScore);
    }
    private void UpdateScore()
    {
        totalScoreText.text = ScoreManager.Instance.GetTotalScore().ToString("00000");
        Debug.Log("About to start coroutine");
        StartCoroutine(DoTextFadeMoveUp());
        StartCoroutine(DoImageFade(flashImage));
    }
    
    private IEnumerator DoTextFadeMoveUp()
    {
        var tempText = TemporaryVariableMaker(effectText);
        if (tempText == null) yield break;
    
        float elapsedTime = 0f;
        Color startColor = tempText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
    
        Vector2 startPosition = tempText.rectTransform.anchoredPosition;

        float randomX = Random.Range(-0.5f, 0.5f); 
        Vector2 moveDirection = new Vector2(randomX, 1f).normalized; 

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
        
            tempText.rectTransform.anchoredPosition = startPosition + (moveDirection * (50 * t));
        
            tempText.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        Destroy(tempText.gameObject);
    }
    
    public IEnumerator DoImageFade(Image fadeImage)
    {
        var tempImage = TemporaryVariableMaker(fadeImage);

        float elapsedTime = 0f;
        
        Color startcolor = tempImage.color;
        Color endcolor = new Color(tempImage.color.r, tempImage.color.g, tempImage.color.b, 0);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            tempImage.color = Color.Lerp(startcolor, endcolor, t);
            
            yield return null;
        }

        Destroy(tempImage.gameObject);
    }
    
    private TextMeshProUGUI TemporaryVariableMaker(TextMeshProUGUI textSample)
    {
        if (textSample == null) return null;
        
        TextMeshProUGUI tempText = Instantiate(textSample, textSample.transform.parent);
        tempText.gameObject.SetActive(true);
        return tempText;
    }
    
    private Image TemporaryVariableMaker(Image imageSample)
    {
        Image tempImage = Instantiate(imageSample, imageSample.transform.parent);
        tempImage.gameObject.SetActive(true);
        return tempImage;
    }
}

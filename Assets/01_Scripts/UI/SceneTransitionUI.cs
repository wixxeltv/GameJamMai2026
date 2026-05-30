using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float fadeInDuration = 1f;
    
    private Coroutine fadeCoroutine;

    void Start()
    {
        LightenScreen();
    }
    
    public void DarkenScreen(string scene)
    {
        fadeCoroutine=StartCoroutine(DoFadeOut(scene));
    }

    public void LightenScreen()
    {
        fadeCoroutine=StartCoroutine(DoFadeIn());
    }
    
    public IEnumerator DoFadeOut(string scene)
    {
        Color startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);;
        Color endColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1); // fully opaque

        float t = 0f;

        while (t < 1f)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            t += Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        SceneManager.LoadScene(scene);
        yield return null;
    }

    public IEnumerator DoFadeIn()
    {
        Color startcolor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);;
        Color endcolor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        float t = 0.0f;

        while (fadeImage.color.a > 0)
        {
            fadeImage.color = Color.Lerp(startcolor, endcolor, t);
            if (t < 1)
            {
                t += Time.deltaTime / fadeInDuration;
            }

            yield return null;
        }

        fadeCoroutine = null;
    }
}
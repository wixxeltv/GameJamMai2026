using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;
    
    [Header("Musics")]
    [SerializeField] public AudioClip attackBGM;
    [SerializeField] public AudioClip bossBGM;
    [SerializeField] public AudioClip mainmenuBGM;
    [SerializeField] public AudioClip tutorialBGM;
    [SerializeField] public AudioClip dialogueBGM;
    [SerializeField] public AudioClip creditsBGM;
    [SerializeField] public AudioClip resolutionBGM;
    
    public static AudioManager Instance;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if(musicSource) musicSource.loop = true;
    }
    
    public void PlaySfx(AudioClip clip, float volume)
    {
        //Allows for SFX to be played in other scripts
        if(sfxSource)
        {
            sfxSource.volume=volume;
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySfx(AudioClip clip)
    {
        //Allows for SFX to be played in other scripts
        if(sfxSource)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        if (musicSource)
        {
            musicSource.volume = volume;
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
    public void StopSfx()
    {
        if(sfxSource) sfxSource.Stop();
    }
    
    public void ChangeBGM(AudioClip clip)
    {
        StartCoroutine(GradualChangeEnumerator(clip));
    }
    
    private IEnumerator GradualChangeEnumerator(AudioClip newClip)
    {
        float t = 0f;
        float startVolume = musicSource.volume;
        
        while (t < 1f)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t);
            t += Time.deltaTime / 1;
            yield return null;
        }

        float t2 = 0f;

        musicSource.clip = newClip;
        musicSource.Play();

        while (t2 < 1f)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t2);
            t2 += Time.deltaTime / 1;
            yield return null;
        }
    }
}